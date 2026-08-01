using HrPanel.Domain.LegacyImport;

namespace HrPanel.Persistence.LegacyImport;
public sealed class LegacyEmployeeImportRow
{
    public long Id { get; private set; }

    // Import tracking metadata
    public Guid BatchId { get; set; }
    public int SourceRowNumber { get; set; }
    public LegacyEmployeeImportStatus ImportStatus { get; private set; }
    public DateTime ReceivedAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public long? ImportedEmployeeId { get; private set; }
    public string? ErrorDetails { get; private set; }
    public string? SourceUserId { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FirstNameFa { get; set; }
    public string? LastNameFa { get; set; }
    public string? EmploymentType { get; set; }
    public string? EmploymentStatus { get; set; }
    public string? Gender { get; set; }
    public string? ManagerUsername { get; set; }
    public string? SupervisorUsername { get; set; }
    public string? QaUsername { get; set; }
    public string? StartWork { get; set; }
    public string? EndWork { get; set; }
    public string? WorkLocation { get; set; }
    public string? Division { get; set; }
    public string? Pilot { get; set; }
    public string? PositionHr { get; set; }
    public string? PositionCr { get; set; }
    public string? JobLevel { get; set; }
    public string? AccessCard { get; set; }
    public string? ArchiveNumber { get; set; }
    public string? FoodCode { get; set; }
    public string? StaffNumber { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    public string? AlternateEmail { get; set; }
    public string? ShiftType { get; set; }
    public string? Description { get; set; }
    public string? SubmitBy { get; set; }
    public string? SubmitDate { get; set; }
    public string? SeniorManager { get; set; }
    public string? WorkDay { get; set; }
    public string? SubDivision { get; set; }
    public string? Department { get; set; }
    public string? Section { get; set; }
    public string? Unit { get; set; }
    public string? Education { get; set; }
    public string? ManagerUsername2 { get; set; }
    public string? ManagerUsername3 { get; set; }
    public string? ManagerUsername4 { get; set; }
    public string? DivisionCr { get; set; }
    public string? SupervisorUsernameCr { get; set; }
    public string? ManagerUsernameCr { get; set; }
    public string? ManagerUsernameCr2 { get; set; }
    public string? ManagerUsernameCr3 { get; set; }
    public string? ManagerUsernameCr4 { get; set; }
    public string? StartWorkFirst { get; set; }
    public string? ContractTerm { get; set; }
    public string? WarningStart { get; set; }
    public string? WarningEnd { get; set; }
    public string? WarningDetail { get; set; }
    public string? FatherName { get; set; }
    public string? FatherNationalId { get; set; }
    public string? Marital { get; set; }
    public string? SpouseName { get; set; }
    public string? SpouseNationalId { get; set; }
    public string? ProvinceWork { get; set; }
    public string? CityWork { get; set; }
    public string? ActivityType { get; set; }
    public string? Td { get; set; }
    public string? Imei { get; set; }
    public string? LegacyId { get; set; }

    public void MarkAsProcessing()
    {
        if (ImportStatus != LegacyEmployeeImportStatus.Pending &&
            ImportStatus != LegacyEmployeeImportStatus.Failed)
        {
            throw new InvalidOperationException($"Row in status '{ImportStatus}' cannot be processed.");
        }

        ImportStatus = LegacyEmployeeImportStatus.Processing;
        ProcessedAtUtc = null;
        ImportedEmployeeId = null;
        ErrorDetails = null;
    }

    public void MarkAsImported(
        long employeeId,
        DateTime processedAtUtc)
    {
        if (ImportStatus != LegacyEmployeeImportStatus.Processing)
        {
            throw new InvalidOperationException("Only a processing row can be marked as imported.");
        }

        if (employeeId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(employeeId));
        }

        ImportStatus = LegacyEmployeeImportStatus.Imported;
        ImportedEmployeeId = employeeId;
        ProcessedAtUtc = processedAtUtc;
        ErrorDetails = null;
    }

    public void MarkAsFailed(string errorDetails,DateTime processedAtUtc)
    {
        if (ImportStatus == LegacyEmployeeImportStatus.Imported ||
            ImportStatus == LegacyEmployeeImportStatus.Skipped)
        {
            throw new InvalidOperationException($"Row in status '{ImportStatus}' cannot be marked as failed.");
        }

        if (string.IsNullOrWhiteSpace(errorDetails))
        {
            throw new ArgumentException("Error details are required.",nameof(errorDetails));
        }

        ImportStatus = LegacyEmployeeImportStatus.Failed;
        ImportedEmployeeId = null;
        ProcessedAtUtc = processedAtUtc;
        ErrorDetails = errorDetails.Trim();
    }

    public void MarkAsSkipped(string reason,DateTime processedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Skip reason is required.",nameof(reason));
        }

        ImportStatus = LegacyEmployeeImportStatus.Skipped;
        ImportedEmployeeId = null;
        ProcessedAtUtc = processedAtUtc;
        ErrorDetails = reason.Trim();
    }
}