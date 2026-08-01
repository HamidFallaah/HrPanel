using FluentValidation;
using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Common.Validation;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Application.Features.Lookups;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Employees;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IValidator<CreateEmployeeDto> _createValidator;
    private readonly IValidator<GetEmployeesDto> _getEmployeesValidator;
    private readonly IValidator<UpdateEmployeePersonalDetailsDto> _personalDetailsValidator;
    private readonly IValidator<UpdateEmployeeNumberDto> _employeeNumberValidator;
    private readonly IValidator<AddEmployeeContactDto> _addContactValidator;
    private readonly IValidator<UpdateEmployeeContactDto> _updateContactValidator;
    private readonly IValidator<AddEmployeeIdentifierDto> _addIdentifierValidator;
    private readonly IValidator<EndEmployeeIdentifierDto> _endIdentifierValidator;
    private readonly IValidator<AddEmployeeEducationDto> _addEducationValidator;
    private readonly IValidator<AddEmployeeDependentDto> _addDependentValidator;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IValidator<CreateEmployeeDto> createValidator,
        IValidator<GetEmployeesDto> getEmployeesValidator,
        IValidator<UpdateEmployeeNumberDto> employeeNumberValidator,
        IValidator<UpdateEmployeePersonalDetailsDto> personalDetailsValidator,
        IValidator<AddEmployeeContactDto> addContactValidator,
        IValidator<UpdateEmployeeContactDto> updateContactValidator,
        IValidator<AddEmployeeIdentifierDto> addIdentifierValidator,
        IValidator<EndEmployeeIdentifierDto> endIdentifierValidator,
        IValidator<AddEmployeeEducationDto> addEducationValidator,
        IValidator<AddEmployeeDependentDto> addDependentValidator)
    {
        _employeeRepository = employeeRepository;
        _createValidator = createValidator;
        _getEmployeesValidator = getEmployeesValidator;
        _employeeNumberValidator = employeeNumberValidator;
        _personalDetailsValidator = personalDetailsValidator;
        _addContactValidator = addContactValidator;
        _updateContactValidator = updateContactValidator;
        _addIdentifierValidator = addIdentifierValidator;
        _endIdentifierValidator = endIdentifierValidator;
        _addEducationValidator = addEducationValidator;
        _addDependentValidator = addDependentValidator;
    }

    public async Task<Result<PagedResult<EmployeeListItemDto>>> GetEmployeesAsync(GetEmployeesDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _getEmployeesValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PagedResult<EmployeeListItemDto>>.Failure(validationResult.ToValidationError());
        }

        var employees = await _employeeRepository.GetPagedAsync(request,cancellationToken);

        return Result<PagedResult<EmployeeListItemDto>>.Success(employees);
    }

    public async Task<Result<long>> CreateEmployeeAsync(CreateEmployeeDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(validationResult.ToValidationError());
        }

        var employeeNumber = request.EmployeeNumber.Trim();

        if (await _employeeRepository.EmployeeNumberExistsAsync(employeeNumber,cancellationToken))
        {
            return Result<long>.Failure(EmployeeErrors.EmployeeNumberAlreadyExists(employeeNumber));
        }

        var nationalCode = Clean(request.NationalCode);

        if (nationalCode is not null &&
            await _employeeRepository.NationalCodeExistsAsync(nationalCode,cancellationToken: cancellationToken))
        {
            return Result<long>.Failure(EmployeeErrors.NationalCodeAlreadyExists(nationalCode));
        }

        var employee = Employee.Create(employeeNumber);
        var personalDetails = EmployeePersonalDetails.Create(request.FirstNameFa,request.LastNameFa,nationalCode);

        personalDetails.UpdateNames(request.FirstName,request.LastName,request.FirstNameFa,request.LastNameFa);

        employee.SetPersonalDetails(personalDetails);

        _employeeRepository.Add(employee);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(employee.Id);
    }

    public async Task<Result<EmployeeDetailsDto>> GetEmployeeDetailsAsync(long employeeId,CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result<EmployeeDetailsDto>.Failure(EmployeeErrors.NotFound(employeeId));
        }

        return Result<EmployeeDetailsDto>.Success(MapDetails(employee));
    }

    public async Task<Result> UpdateEmployeeNumberAsync(long employeeId,UpdateEmployeeNumberDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _employeeNumberValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var employeeNumber = request.EmployeeNumber.Trim();

        if (!string.Equals(employee.EmployeeNumber,employeeNumber,StringComparison.OrdinalIgnoreCase) &&
            await _employeeRepository.EmployeeNumberExistsAsync(employeeNumber,cancellationToken))
        {
            return Result.Failure(EmployeeErrors.EmployeeNumberAlreadyExists(employeeNumber));
        }

        employee.ChangeEmployeeNumber(employeeNumber);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdatePersonalDetailsAsync(long employeeId,UpdateEmployeePersonalDetailsDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _personalDetailsValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var nationalCode = Clean(request.NationalCode);

        if (nationalCode is not null && await _employeeRepository.NationalCodeExistsAsync(nationalCode,employeeId,cancellationToken))
        {
            return Result.Failure(EmployeeErrors.NationalCodeAlreadyExists(nationalCode));
        }

        var personalDetails = employee.PersonalDetails;

        if (personalDetails is null)
        {
            personalDetails = EmployeePersonalDetails.Create(request.FirstNameFa,request.LastNameFa,nationalCode);

            employee.SetPersonalDetails(personalDetails);
        }

        personalDetails.UpdateNames(
            request.FirstName,
            request.LastName,
            request.FirstNameFa,
            request.LastNameFa);
        personalDetails.SetNationalCode(nationalCode);
        personalDetails.UpdatePersonalInformation(
            request.BirthDate,
            request.BirthPlace,
            request.Gender,
            request.MaritalStatus,
            request.FatherName,
            request.FatherNationalCode);

        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public Task<Result> ActivateEmployeeAsync(long employeeId,CancellationToken cancellationToken = default)
    {
        return ChangeEmployeeStatusAsync(employeeId,true,cancellationToken);
    }

    public Task<Result> DeactivateEmployeeAsync(long employeeId,CancellationToken cancellationToken = default)
    {
        return ChangeEmployeeStatusAsync(employeeId,false,cancellationToken);
    }

    public async Task<Result<long>> AddContactAsync(long employeeId,AddEmployeeContactDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _addContactValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result<long>.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var contact = EmployeeContact.Create(request.Type,request.Value,request.IsPrimary);

        employee.AddContact(contact);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(contact.Id);
    }

    public async Task<Result> UpdateContactAsync(long employeeId,long contactId,UpdateEmployeeContactDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateContactValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var contact = employee.Contacts.SingleOrDefault(item => item.Id == contactId);

        if (contact is null)
        {
            return Result.Failure(EmployeeErrors.ContactNotFound(contactId));
        }

        employee.UpdateContact(contact,request.Value);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveContactAsync(long employeeId,long contactId,CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var contact = employee.Contacts.SingleOrDefault(item => item.Id == contactId);

        if (contact is null)
        {
            return Result.Failure(EmployeeErrors.ContactNotFound(contactId));
        }

        employee.RemoveContact(contact);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SelectPrimaryContactAsync(long employeeId,long contactId,CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var contact = employee.Contacts.SingleOrDefault(item => item.Id == contactId);

        if (contact is null)
        {
            return Result.Failure(EmployeeErrors.ContactNotFound(contactId));
        }

        employee.MarkContactAsPrimary(contact);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<long>> AddIdentifierAsync(long employeeId,AddEmployeeIdentifierDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _addIdentifierValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result<long>.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var value = request.Value.Trim();

        if (await _employeeRepository.ActiveIdentifierExistsAsync(request.Type,value,cancellationToken))
        {
            return Result<long>.Failure(EmployeeErrors.ActiveIdentifierAlreadyExists(value));
        }

        var identifier = EmployeeIdentifier.Create(request.Type,value,request.EffectiveFrom);

        employee.AddIdentifier(identifier);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(identifier.Id);
    }

    public async Task<Result> EndIdentifierAsync(long employeeId,long identifierId,EndEmployeeIdentifierDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _endIdentifierValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var identifier = employee.Identifiers.SingleOrDefault(item => item.Id == identifierId);

        if (identifier is null)
        {
            return Result.Failure(EmployeeErrors.IdentifierNotFound(identifierId));
        }

        if (identifier.EffectiveTo.HasValue)
        {
            return Result.Failure(Error.Conflict("Employees.IdentifierAlreadyEnded","این شناسه قبلاً پایان یافته است"));
        }

        if (identifier.EffectiveFrom.HasValue && request.EffectiveTo < identifier.EffectiveFrom.Value)
        {
            return Result.Failure(Error.Failure("Employees.IdentifierEndBeforeStart","تاریخ پایان شناسه نمی‌تواند قبل از تاریخ شروع آن باشد"));
        }

        employee.EndIdentifier(identifier,request.EffectiveTo);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<long>> AddEducationAsync(long employeeId,AddEmployeeEducationDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _addEducationValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result<long>.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var education = string.IsNullOrWhiteSpace(request.DegreeTitle)
            ? EmployeeEducation.CreateFieldOfStudyOnly(
                request.FieldOfStudy!,
                request.InstitutionName)
            : EmployeeEducation.Create(
                request.DegreeTitle!,
                request.FieldOfStudy,
                request.InstitutionName);

        education.SetGraduationDate(request.GraduationDate);

        if (request.IsHighestDegree)
        {
            education.MarkAsHighestDegree();
        }

        employee.AddEducation(education);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(education.Id);
    }

    public async Task<Result> UpdateEducationAsync(long employeeId,long educationId,AddEmployeeEducationDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _addEducationValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var education = employee.EducationRecords.SingleOrDefault(item => item.Id == educationId);

        if (education is null)
        {
            return Result.Failure(EmployeeErrors.EducationNotFound(educationId));
        }

        employee.UpdateEducation(
            education,
            request.DegreeTitle,
            request.FieldOfStudy,
            request.InstitutionName,
            request.GraduationDate,
            request.IsHighestDegree);

        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SelectHighestEducationAsync(long employeeId,long educationId,CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var education = employee.EducationRecords.SingleOrDefault(
            item => item.Id == educationId);

        if (education is null)
        {
            return Result.Failure(EmployeeErrors.EducationNotFound(educationId));
        }

        employee.MarkEducationAsHighest(education);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveEducationAsync(long employeeId,long educationId,CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var education = employee.EducationRecords.SingleOrDefault(item => item.Id == educationId);

        if (education is null)
        {
            return Result.Failure(EmployeeErrors.EducationNotFound(educationId));
        }

        employee.RemoveEducation(education);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<long>> AddDependentAsync(long employeeId,AddEmployeeDependentDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _addDependentValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<long>.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result<long>.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var dependent = EmployeeDependent.Create(request.FullName,request.RelationshipType);

        dependent.UpdateDetails(request.NationalCode,request.BirthDate);

        if (request.IsEmergencyContact)
        {
            dependent.SetAsEmergencyContact(request.EmergencyPhone!);
        }

        employee.AddDependent(dependent);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.Success(dependent.Id);
    }

    public async Task<Result> UpdateDependentAsync(long employeeId,long dependentId,AddEmployeeDependentDto request,CancellationToken cancellationToken = default)
    {
        var validationResult = await _addDependentValidator.ValidateAsync( request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ToValidationError());
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var dependent = employee.Dependents.SingleOrDefault(item => item.Id == dependentId);

        if (dependent is null)
        {
            return Result.Failure(EmployeeErrors.DependentNotFound(dependentId));
        }

        employee.UpdateDependent(
            dependent,
            request.FullName,
            request.RelationshipType,
            request.NationalCode,
            request.BirthDate,
            request.IsEmergencyContact,
            request.EmergencyPhone);

        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RemoveDependentAsync(long employeeId,long dependentId,CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        var dependent = employee.Dependents.SingleOrDefault(item => item.Id == dependentId);

        if (dependent is null)
        {
            return Result.Failure(EmployeeErrors.DependentNotFound(dependentId));
        }

        employee.RemoveDependent(dependent);
        await _employeeRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> ChangeEmployeeStatusAsync(long employeeId,bool isActive,CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId,cancellationToken);

        if (employee is null)
        {
            return Result.Failure(EmployeeErrors.NotFound(employeeId));
        }

        if (isActive)
        {
            employee.Activate();
        }
        else
        {
            employee.Deactivate();
        }

        await _employeeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static EmployeeDetailsDto MapDetails(Employee employee)
    {
        var personalDetails = employee.PersonalDetails is null
            ? null
            : new EmployeePersonalDetailsDto(
                employee.PersonalDetails.FirstName,
                employee.PersonalDetails.LastName,
                employee.PersonalDetails.FirstNameFa,
                employee.PersonalDetails.LastNameFa,
                employee.PersonalDetails.NationalCode,
                employee.PersonalDetails.FatherName,
                employee.PersonalDetails.FatherNationalCode,
                employee.PersonalDetails.BirthDate,
                employee.PersonalDetails.BirthPlace,
                (short)employee.PersonalDetails.Gender,
                employee.PersonalDetails.Gender.ToString(),
                EmployeeLookupNames.GetDisplayName(employee.PersonalDetails.Gender),
                (short)employee.PersonalDetails.MaritalStatus,
                employee.PersonalDetails.MaritalStatus.ToString(),
                EmployeeLookupNames.GetDisplayName(employee.PersonalDetails.MaritalStatus));

        var contacts = employee.Contacts
            .OrderBy(contact => contact.Type)
            .ThenByDescending(contact => contact.IsPrimary)
            .Select(contact => new EmployeeContactDto(
                contact.Id,
                (short)contact.Type,
                contact.Type.ToString(),
                EmployeeLookupNames.GetDisplayName(contact.Type),
                contact.Value,
                contact.IsPrimary,
                contact.CreatedAt,
                contact.ModifiedAt))
            .ToArray();

        var identifiers = employee.Identifiers
            .OrderBy(identifier => identifier.Type)
            .ThenByDescending(identifier => identifier.EffectiveFrom)
            .Select(identifier => new EmployeeIdentifierDto(
                identifier.Id,
                (short)identifier.Type,
                identifier.Type.ToString(),
                EmployeeLookupNames.GetDisplayName(identifier.Type),
                identifier.Value,
                identifier.EffectiveFrom,
                identifier.EffectiveTo,
                !identifier.EffectiveTo.HasValue,
                identifier.CreatedAt,
                identifier.ModifiedAt))
            .ToArray();

        var educationRecords = employee.EducationRecords
            .OrderByDescending(education => education.IsHighestDegree)
            .ThenByDescending(education => education.GraduationDate)
            .Select(education => new EmployeeEducationDto(
                education.Id,
                education.DegreeTitle,
                education.FieldOfStudy,
                education.InstitutionName,
                education.GraduationDate,
                education.IsHighestDegree,
                education.CreatedAt,
                education.ModifiedAt))
            .ToArray();

        var dependents = employee.Dependents
            .OrderBy(dependent => dependent.RelationshipType)
            .ThenBy(dependent => dependent.FullName)
            .Select(dependent => new EmployeeDependentDto(
                dependent.Id,
                dependent.FullName,
                dependent.NationalCode,
                dependent.BirthDate,
                (short)dependent.RelationshipType,
                dependent.RelationshipType.ToString(),
                EmployeeLookupNames.GetDisplayName(dependent.RelationshipType),
                dependent.IsEmergencyContact,
                dependent.EmergencyPhone,
                dependent.CreatedAt,
                dependent.ModifiedAt))
            .ToArray();

        return new EmployeeDetailsDto(
            employee.Id,
            employee.EmployeeNumber,
            employee.LegacyUserId,
            employee.LegacyGuid,
            employee.IsActive,
            personalDetails,
            contacts,
            identifiers,
            educationRecords,
            dependents,
            employee.CreatedAt,
            employee.ModifiedAt);
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null: value.Trim();
    }
}
