using FluentValidation;
using HrPanel.Application.Dtos.Employments;

namespace HrPanel.Application.Features.Employments.Validators;

public sealed class GetEmploymentsDtoValidator : AbstractValidator<GetEmploymentsDto>
{
    public GetEmploymentsDtoValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.EmployeeId).GreaterThan(0).When(x => x.EmployeeId.HasValue);
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1,100);
    }
}

public sealed class StartEmploymentDtoValidator : AbstractValidator<StartEmploymentDto>
{
    public StartEmploymentDtoValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.EmploymentTypeId).GreaterThan((short)0);
        RuleFor(x => x.EmploymentStatusId).GreaterThan((short)0);
        RuleFor(x => x.WorkTimeTypeId).GreaterThan((short)0).When(x => x.WorkTimeTypeId.HasValue);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.ContractTermMonths).InclusiveBetween((short)1,(short)120).When(x => x.ContractTermMonths.HasValue);
    }
}

public sealed class ChangeEmploymentStatusDtoValidator : AbstractValidator<ChangeEmploymentStatusDto>
{
    public ChangeEmploymentStatusDtoValidator()
    {
        RuleFor(x => x.EmploymentStatusId).GreaterThan((short)0);
    }
}

public sealed class ChangeWorkTimeTypeDtoValidator : AbstractValidator<ChangeWorkTimeTypeDto>
{
    public ChangeWorkTimeTypeDtoValidator()
    {
        RuleFor(x => x.WorkTimeTypeId).GreaterThan((short)0).When(x => x.WorkTimeTypeId.HasValue);
    }
}

public sealed class EndEmploymentDtoValidator : AbstractValidator<EndEmploymentDto>
{
    public EndEmploymentDtoValidator()
    {
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.EmploymentStatusId).GreaterThan((short)0);
        RuleFor(x => x.Reason).MaximumLength(1000);
    }
}

public sealed class AddEmployeeAssignmentDtoValidator : AbstractValidator<AddEmployeeAssignmentDto>
{
    public AddEmployeeAssignmentDtoValidator()
    {
        RuleFor(x => x.Context).IsInEnum();
        RuleFor(x => x.EffectiveFrom).NotEmpty();
        RuleFor(x => x.OrganizationUnitId).GreaterThan(0).When(x => x.OrganizationUnitId.HasValue);
        RuleFor(x => x.PositionId).GreaterThan(0).When(x => x.PositionId.HasValue);
        RuleFor(x => x.JobLevelId).GreaterThan((short)0).When(x => x.JobLevelId.HasValue);
        RuleFor(x => x.WorkLocationId).GreaterThan(0).When(x => x.WorkLocationId.HasValue);
        RuleFor(x => x).Must(x => x.OrganizationUnitId.HasValue || x.PositionId.HasValue || x.JobLevelId.HasValue || x.WorkLocationId.HasValue)
            .WithMessage("حداقل یکی از اطلاعات تخصیص الزامی است");
    }
}

public sealed class EndAssignmentDtoValidator : AbstractValidator<EndAssignmentDto>
{
    public EndAssignmentDtoValidator()
    {
        RuleFor(x => x.EffectiveTo).NotEmpty();
    }
}

public sealed class AssignOperationalGroupDtoValidator : AbstractValidator<AssignOperationalGroupDto>
{
    public AssignOperationalGroupDtoValidator()
    {
        RuleFor(x => x.OperationalGroupId).GreaterThan(0);
        RuleFor(x => x.EffectiveFrom).NotEmpty();
    }
}

public sealed class AddEmployeeRelationshipDtoValidator : AbstractValidator<AddEmployeeRelationshipDto>
{
    public AddEmployeeRelationshipDtoValidator()
    {
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Context).IsInEnum();
        RuleFor(x => x.EffectiveFrom).NotEmpty();
        RuleFor(x => x.RelatedEmployeeId).GreaterThan(0).When(x => x.RelatedEmployeeId.HasValue);
        RuleFor(x => x.RelatedExternalPersonId).GreaterThan(0).When(x => x.RelatedExternalPersonId.HasValue);
        RuleFor(x => x).Must(x => x.RelatedEmployeeId.HasValue ^ x.RelatedExternalPersonId.HasValue)
            .WithMessage("رابطه باید دقیقاً یک کارمند یا شخص خارجی داشته باشد");
    }
}

public sealed class CreateExternalPersonDtoValidator : AbstractValidator<CreateExternalPersonDto>
{
    public CreateExternalPersonDtoValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LegacyUsername).MaximumLength(128);
    }
}

public sealed class UpdateExternalPersonDtoValidator : AbstractValidator<UpdateExternalPersonDto>
{
    public UpdateExternalPersonDtoValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LegacyUsername).MaximumLength(128);
    }
}

public sealed class AddDisciplinaryActionDtoValidator : AbstractValidator<AddDisciplinaryActionDto>
{
    public AddDisciplinaryActionDtoValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x).Must(x => !x.EndDate.HasValue || x.EndDate.Value >= x.StartDate)
            .WithMessage("تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد");
        RuleFor(x => x.Details).NotEmpty().MaximumLength(2000);
    }
}

public sealed class CloseDisciplinaryActionDtoValidator : AbstractValidator<CloseDisciplinaryActionDto>
{
    public CloseDisciplinaryActionDtoValidator()
    {
        RuleFor(x => x.EndDate).NotEmpty();
    }
}
