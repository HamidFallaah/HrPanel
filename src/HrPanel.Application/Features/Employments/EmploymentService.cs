using FluentValidation;
using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Common.Validation;
using HrPanel.Application.Dtos.Employments;
using HrPanel.Domain.Employment;

namespace HrPanel.Application.Features.Employments;

public sealed class EmploymentService : IEmploymentService
{
    private readonly IEmploymentRepository _repository;
    private readonly IValidator<GetEmploymentsDto> _queryValidator;
    private readonly IValidator<StartEmploymentDto> _startValidator;
    private readonly IValidator<ChangeEmploymentStatusDto> _statusValidator;
    private readonly IValidator<ChangeWorkTimeTypeDto> _workTimeValidator;
    private readonly IValidator<EndEmploymentDto> _endEmploymentValidator;
    private readonly IValidator<AddEmployeeAssignmentDto> _assignmentValidator;
    private readonly IValidator<EndAssignmentDto> _endAssignmentValidator;
    private readonly IValidator<AssignOperationalGroupDto> _groupValidator;
    private readonly IValidator<AddEmployeeRelationshipDto> _relationshipValidator;
    private readonly IValidator<CreateExternalPersonDto> _createExternalPersonValidator;
    private readonly IValidator<UpdateExternalPersonDto> _updateExternalPersonValidator;
    private readonly IValidator<AddDisciplinaryActionDto> _disciplinaryValidator;
    private readonly IValidator<CloseDisciplinaryActionDto> _closeDisciplinaryValidator;

    public EmploymentService(
        IEmploymentRepository repository,
        IValidator<GetEmploymentsDto> queryValidator,
        IValidator<StartEmploymentDto> startValidator,
        IValidator<ChangeEmploymentStatusDto> statusValidator,
        IValidator<ChangeWorkTimeTypeDto> workTimeValidator,
        IValidator<EndEmploymentDto> endEmploymentValidator,
        IValidator<AddEmployeeAssignmentDto> assignmentValidator,
        IValidator<EndAssignmentDto> endAssignmentValidator,
        IValidator<AssignOperationalGroupDto> groupValidator,
        IValidator<AddEmployeeRelationshipDto> relationshipValidator,
        IValidator<CreateExternalPersonDto> createExternalPersonValidator,
        IValidator<UpdateExternalPersonDto> updateExternalPersonValidator,
        IValidator<AddDisciplinaryActionDto> disciplinaryValidator,
        IValidator<CloseDisciplinaryActionDto> closeDisciplinaryValidator)
    {
        _repository = repository;
        _queryValidator = queryValidator;
        _startValidator = startValidator;
        _statusValidator = statusValidator;
        _workTimeValidator = workTimeValidator;
        _endEmploymentValidator = endEmploymentValidator;
        _assignmentValidator = assignmentValidator;
        _endAssignmentValidator = endAssignmentValidator;
        _groupValidator = groupValidator;
        _relationshipValidator = relationshipValidator;
        _createExternalPersonValidator = createExternalPersonValidator;
        _updateExternalPersonValidator = updateExternalPersonValidator;
        _disciplinaryValidator = disciplinaryValidator;
        _closeDisciplinaryValidator = closeDisciplinaryValidator;
    }

    public async Task<Result<PagedResult<EmploymentListItemDto>>> GetEmploymentsAsync(GetEmploymentsDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _queryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<PagedResult<EmploymentListItemDto>>.Failure(validation.ToValidationError());
        return Result<PagedResult<EmploymentListItemDto>>.Success(await _repository.GetPagedAsync(request,cancellationToken));
    }

    public async Task<Result<EmploymentDetailsDto>> GetEmploymentAsync(long employmentId,CancellationToken cancellationToken = default)
    {
        var employment = await _repository.GetDetailsAsync(employmentId,cancellationToken);
        return employment is null ? Result<EmploymentDetailsDto>.Failure(EmploymentErrors.NotFound(employmentId)) : Result<EmploymentDetailsDto>.Success(employment);
    }

    public async Task<Result<long>> StartEmploymentAsync(StartEmploymentDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _startValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        if (!await _repository.EmployeeExistsAsync(request.EmployeeId,cancellationToken)) return Result<long>.Failure(EmploymentErrors.EmployeeNotFound(request.EmployeeId));
        if (await _repository.CurrentEmploymentExistsAsync(request.EmployeeId,cancellationToken)) return Result<long>.Failure(EmploymentErrors.CurrentEmploymentExists(request.EmployeeId));
        var references = await ValidateEmploymentReferencesAsync(request.EmploymentTypeId,request.EmploymentStatusId,request.WorkTimeTypeId,cancellationToken);
        if (references.IsFailure) return Result<long>.Failure(references.Error);

        var employment = Employment.Start(request.EmployeeId,request.EmploymentTypeId,request.EmploymentStatusId,request.StartDate,request.ContractTermMonths,request.WorkTimeTypeId);
        _repository.Add(employment);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(employment.Id);
    }

    public async Task<Result> ChangeStatusAsync(long employmentId,ChangeEmploymentStatusDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _statusValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var employment = await _repository.GetByIdAsync(employmentId,cancellationToken);
        if (employment is null) return Result.Failure(EmploymentErrors.NotFound(employmentId));
        if (!await _repository.EmploymentStatusExistsAsync(request.EmploymentStatusId,cancellationToken)) return Result.Failure(EmploymentErrors.ReferenceNotFound("وضعیت استخدام"));
        employment.ChangeStatus(request.EmploymentStatusId);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangeWorkTimeTypeAsync(long employmentId,ChangeWorkTimeTypeDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _workTimeValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var employment = await _repository.GetByIdAsync(employmentId,cancellationToken);
        if (employment is null) return Result.Failure(EmploymentErrors.NotFound(employmentId));
        if (request.WorkTimeTypeId.HasValue && !await _repository.WorkTimeTypeExistsAsync(request.WorkTimeTypeId.Value,cancellationToken)) return Result.Failure(EmploymentErrors.ReferenceNotFound("نوع ساعت کاری"));
        if (request.WorkTimeTypeId.HasValue) employment.ChangeWorkTimeType(request.WorkTimeTypeId.Value); else employment.ClearWorkTimeType();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> EndEmploymentAsync(long employmentId,EndEmploymentDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _endEmploymentValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var employment = await _repository.GetByIdAsync(employmentId,cancellationToken);
        if (employment is null) return Result.Failure(EmploymentErrors.NotFound(employmentId));
        if (employment.EndDate.HasValue) return Result.Failure(Error.Conflict("Employments.AlreadyEnded","استخدام قبلاً پایان یافته است"));
        if (request.EndDate < employment.StartDate) return Result.Failure(Error.Failure("Employments.EndBeforeStart","تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد"));
        if (!await _repository.EmploymentStatusExistsAsync(request.EmploymentStatusId,cancellationToken)) return Result.Failure(EmploymentErrors.ReferenceNotFound("وضعیت پایان استخدام"));
        var groupAssignments = await _repository.GetCurrentOperationalGroupAssignmentsAsync(
            employmentId,
            cancellationToken);

        var scheduleAssignment = await _repository.GetCurrentScheduleAssignmentAsync(
            employmentId,
            cancellationToken);

        if (groupAssignments.Any(item => item.EffectiveFrom > request.EndDate) ||
            (scheduleAssignment is not null &&
             scheduleAssignment.EffectiveFrom > request.EndDate))
        {
            return Result.Failure(Error.Failure("Employments.EndBeforeRelatedAssignment","تاریخ پایان استخدام نمی‌تواند قبل از شروع تخصیص فعال باشد"));
        }

        employment.End(request.EndDate,request.EmploymentStatusId,request.Reason);

        foreach (var assignment in groupAssignments)
        {
            assignment.End(request.EndDate);
        }

        scheduleAssignment?.End(request.EndDate);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<long>> AddAssignmentAsync(long employmentId,AddEmployeeAssignmentDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _assignmentValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        var employment = await _repository.GetByIdAsync(employmentId,cancellationToken);
        if (employment is null) return Result<long>.Failure(EmploymentErrors.NotFound(employmentId));
        if (!employment.IsCurrent) return Result<long>.Failure(Error.Conflict("Employments.Ended","استخدام پایان‌یافته قابل تغییر نیست"));
        if (employment.Assignments.Any(item => item.Context == request.Context && item.IsCurrent)) return Result<long>.Failure(EmploymentErrors.CurrentAssignmentExists());
        if (!await _repository.AssignmentReferencesExistAsync(request,cancellationToken)) return Result<long>.Failure(EmploymentErrors.ReferenceNotFound("تخصیص سازمانی"));
        var assignment = EmployeeAssignment.Create(request.Context,request.EffectiveFrom,request.OrganizationUnitId,request.PositionId,request.JobLevelId,request.WorkLocationId);
        employment.AddAssignment(assignment);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(assignment.Id);
    }

    public async Task<Result> EndAssignmentAsync(long employmentId,long assignmentId,EndAssignmentDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _endAssignmentValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var employment = await _repository.GetByIdAsync(employmentId,cancellationToken);
        if (employment is null) return Result.Failure(EmploymentErrors.NotFound(employmentId));
        var assignment = employment.Assignments.SingleOrDefault(item => item.Id == assignmentId);
        if (assignment is null) return Result.Failure(EmploymentErrors.AssignmentNotFound(assignmentId));
        if (!assignment.IsCurrent) return Result.Failure(Error.Conflict("Employments.AssignmentEnded","تخصیص قبلاً پایان یافته است"));
        if (request.EffectiveTo < assignment.EffectiveFrom) return Result.Failure(Error.Failure("Employments.InvalidAssignmentEnd","تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد"));
        assignment.End(request.EffectiveTo);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<long>> AssignOperationalGroupAsync(long employmentId,AssignOperationalGroupDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _groupValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        var employment = await _repository.GetByIdAsync(employmentId,cancellationToken);
        if (employment is null || !employment.IsCurrent) return Result<long>.Failure(EmploymentErrors.NotFound(employmentId));
        if (!await _repository.OperationalGroupExistsAsync(request.OperationalGroupId,cancellationToken)) return Result<long>.Failure(EmploymentErrors.ReferenceNotFound("گروه عملیاتی"));
        var current = await _repository.GetCurrentOperationalGroupAssignmentsAsync(employmentId,cancellationToken);
        if (current.Any(item => item.OperationalGroupId == request.OperationalGroupId)) return Result<long>.Failure(Error.Conflict("Employments.GroupAlreadyAssigned","این گروه عملیاتی قبلاً فعال است"));
        if (request.IsPrimary) foreach (var item in current.Where(item => item.IsPrimary)) item.MakeSecondary();
        var assignment = EmployeeOperationalGroupAssignment.Create(employmentId,request.OperationalGroupId,request.EffectiveFrom,request.IsPrimary);
        _repository.Add(assignment);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(assignment.Id);
    }
    public async Task<Result> SelectPrimaryOperationalGroupAsync(long employmentId,long assignmentId,CancellationToken cancellationToken = default)
    {
        var assignment = await _repository.GetOperationalGroupAssignmentAsync(employmentId,assignmentId,cancellationToken);
        if (assignment is null || !assignment.IsCurrent) return Result.Failure(EmploymentErrors.OperationalGroupAssignmentNotFound(assignmentId));
        var current = await _repository.GetCurrentOperationalGroupAssignmentsAsync(employmentId,cancellationToken);
        foreach (var item in current) item.MakeSecondary();
        assignment.MakePrimary();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> EndOperationalGroupAssignmentAsync(long employmentId,long assignmentId,EndAssignmentDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _endAssignmentValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var assignment = await _repository.GetOperationalGroupAssignmentAsync(employmentId,assignmentId,cancellationToken);
        if (assignment is null) return Result.Failure(EmploymentErrors.OperationalGroupAssignmentNotFound(assignmentId));
        if (!assignment.IsCurrent) return Result.Failure(Error.Conflict("Employments.GroupAssignmentEnded","تخصیص گروه قبلاً پایان یافته است"));
        if (request.EffectiveTo < assignment.EffectiveFrom) return Result.Failure(Error.Failure("Employments.InvalidGroupEnd","تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد"));
        assignment.End(request.EffectiveTo);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result<long>> AddRelationshipAsync(long employeeId,AddEmployeeRelationshipDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _relationshipValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        if (!await _repository.EmployeeExistsAsync(employeeId,cancellationToken)) return Result<long>.Failure(EmploymentErrors.EmployeeNotFound(employeeId));
        if (request.RelatedEmployeeId == employeeId) return Result<long>.Failure(Error.Failure("Employments.SelfRelationship","کارمند نمی‌تواند با خودش رابطه سازمانی داشته باشد"));
        if (request.RelatedEmployeeId.HasValue && !await _repository.EmployeeExistsAsync(request.RelatedEmployeeId.Value,cancellationToken)) return Result<long>.Failure(EmploymentErrors.EmployeeNotFound(request.RelatedEmployeeId.Value));
        if (request.RelatedExternalPersonId.HasValue && !await _repository.ExternalPersonExistsAsync(request.RelatedExternalPersonId.Value,cancellationToken)) return Result<long>.Failure(EmploymentErrors.ExternalPersonNotFound(request.RelatedExternalPersonId.Value));
        if (await _repository.CurrentRelationshipExistsAsync(employeeId,request.Type,request.Context,cancellationToken)) return Result<long>.Failure(Error.Conflict("Employments.RelationshipExists","برای این نوع و حوزه یک رابطه فعال وجود دارد"));

        var relationship = request.RelatedEmployeeId.HasValue
            ? EmployeeRelationship.ForEmployee(employeeId,request.Type,request.Context,request.RelatedEmployeeId.Value,request.EffectiveFrom)
            : EmployeeRelationship.ForExternalPerson(employeeId,request.Type,request.Context,request.RelatedExternalPersonId!.Value,request.EffectiveFrom);
        _repository.Add(relationship);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(relationship.Id);
    }
    public async Task<Result> EndRelationshipAsync(long employeeId,long relationshipId,EndAssignmentDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _endAssignmentValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var relationship = await _repository.GetRelationshipAsync(employeeId,relationshipId,cancellationToken);
        if (relationship is null) return Result.Failure(EmploymentErrors.RelationshipNotFound(relationshipId));
        if (!relationship.IsCurrent) return Result.Failure(Error.Conflict("Employments.RelationshipEnded","رابطه قبلاً پایان یافته است"));
        if (request.EffectiveTo < relationship.EffectiveFrom) return Result.Failure(Error.Failure("Employments.InvalidRelationshipEnd","تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد"));
        relationship.End(request.EffectiveTo);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result<IReadOnlyCollection<ExternalPersonDto>>> GetExternalPersonsAsync(string? search,bool? isActive,CancellationToken cancellationToken = default)
    {
        if (search?.Length > 100) return Result<IReadOnlyCollection<ExternalPersonDto>>.Failure(Error.Failure("Employments.SearchTooLong","عبارت جستجو بیش از حد مجاز است"));
        return Result<IReadOnlyCollection<ExternalPersonDto>>.Success(await _repository.GetExternalPersonsAsync(search,isActive,cancellationToken));
    }
    public async Task<Result<long>> CreateExternalPersonAsync(CreateExternalPersonDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _createExternalPersonValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        var username = Clean(request.LegacyUsername);
        if (username is not null && await _repository.ExternalUsernameExistsAsync(username,cancellationToken: cancellationToken)) return Result<long>.Failure(Error.Conflict("Employments.ExternalUsernameExists","نام کاربری شخص خارجی قبلاً ثبت شده است"));
        var person = ExternalPerson.Create(request.DisplayName,username);
        _repository.Add(person);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(person.Id);
    }
    public async Task<Result> UpdateExternalPersonAsync(long externalPersonId,UpdateExternalPersonDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _updateExternalPersonValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var person = await _repository.GetExternalPersonAsync(externalPersonId,cancellationToken);
        if (person is null) return Result.Failure(EmploymentErrors.ExternalPersonNotFound(externalPersonId));
        var username = Clean(request.LegacyUsername);
        if (username is not null && await _repository.ExternalUsernameExistsAsync(username,externalPersonId,cancellationToken)) return Result.Failure(Error.Conflict("Employments.ExternalUsernameExists","نام کاربری شخص خارجی قبلاً ثبت شده است"));
        person.Update(request.DisplayName,username);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> ChangeExternalPersonStatusAsync(long externalPersonId,bool isActive,CancellationToken cancellationToken = default)
    {
        var person = await _repository.GetExternalPersonAsync(externalPersonId,cancellationToken);
        if (person is null) return Result.Failure(EmploymentErrors.ExternalPersonNotFound(externalPersonId));
        if (isActive) person.Activate(); else person.Deactivate();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result<long>> AddDisciplinaryActionAsync(AddDisciplinaryActionDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _disciplinaryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        if (!await _repository.EmployeeExistsAsync(request.EmployeeId,cancellationToken)) return Result<long>.Failure(EmploymentErrors.EmployeeNotFound(request.EmployeeId));
        var action = DisciplinaryAction.Create(request.EmployeeId,request.StartDate,request.EndDate,request.Details);
        _repository.Add(action);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(action.Id);
    }
    public async Task<Result> CloseDisciplinaryActionAsync(long employeeId,long actionId,CloseDisciplinaryActionDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _closeDisciplinaryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var action = await _repository.GetDisciplinaryActionAsync(employeeId,actionId,cancellationToken);
        if (action is null) return Result.Failure(EmploymentErrors.DisciplinaryActionNotFound(actionId));
        if (action.IsClosed) return Result.Failure(Error.Conflict("Employments.DisciplinaryActionClosed","اقدام انضباطی قبلاً بسته شده است"));
        if (request.EndDate < action.StartDate) return Result.Failure(Error.Failure("Employments.InvalidDisciplinaryEnd","تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد"));
        action.Close(request.EndDate);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    private async Task<Result> ValidateEmploymentReferencesAsync(short employmentTypeId,short employmentStatusId,short? workTimeTypeId,CancellationToken cancellationToken)
    {
        if (!await _repository.EmploymentTypeExistsAsync(employmentTypeId,cancellationToken)) return Result.Failure(EmploymentErrors.ReferenceNotFound("نوع استخدام"));
        if (!await _repository.EmploymentStatusExistsAsync(employmentStatusId,cancellationToken)) return Result.Failure(EmploymentErrors.ReferenceNotFound("وضعیت استخدام"));
        if (workTimeTypeId.HasValue && !await _repository.WorkTimeTypeExistsAsync(workTimeTypeId.Value,cancellationToken)) return Result.Failure(EmploymentErrors.ReferenceNotFound("نوع ساعت کاری"));
        return Result.Success();
    }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
