namespace HrPanel.Application.Dtos.Employees;

public sealed record EmployeeDetailsDto(
    long Id,
    string EmployeeNumber,
    string? LegacyUserId,
    Guid? LegacyGuid,
    bool IsActive,
    EmployeePersonalDetailsDto? PersonalDetails,
    IReadOnlyCollection<EmployeeContactDto> Contacts,
    IReadOnlyCollection<EmployeeIdentifierDto> Identifiers,
    IReadOnlyCollection<EmployeeEducationDto> EducationRecords,
    IReadOnlyCollection<EmployeeDependentDto> Dependents,
    DateTime CreatedAt,
    DateTime? ModifiedAt);

public sealed record EmployeePersonalDetailsDto(
    string? FirstName,
    string? LastName,
    string FirstNameFa,
    string LastNameFa,
    string? NationalCode,
    string? FatherName,
    string? FatherNationalCode,
    DateOnly? BirthDate,
    string? BirthPlace,
    short GenderId,
    string GenderName,
    string GenderDisplayName,
    short MaritalStatusId,
    string MaritalStatusName,
    string MaritalStatusDisplayName);

public sealed record EmployeeContactDto(
    long Id,
    short TypeId,
    string TypeName,
    string TypeDisplayName,
    string Value,
    bool IsPrimary,
    DateTime CreatedAt,
    DateTime? ModifiedAt);

public sealed record EmployeeIdentifierDto(
    long Id,
    short TypeId,
    string TypeName,
    string TypeDisplayName,
    string Value,
    DateOnly? EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? ModifiedAt);

public sealed record EmployeeEducationDto(
    long Id,
    string? DegreeTitle,
    string? FieldOfStudy,
    string? InstitutionName,
    DateOnly? GraduationDate,
    bool IsHighestDegree,
    DateTime CreatedAt,
    DateTime? ModifiedAt);

public sealed record EmployeeDependentDto(
    long Id,
    string FullName,
    string? NationalCode,
    DateOnly? BirthDate,
    short RelationshipTypeId,
    string RelationshipTypeName,
    string RelationshipTypeDisplayName,
    bool IsEmergencyContact,
    string? EmergencyPhone,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
