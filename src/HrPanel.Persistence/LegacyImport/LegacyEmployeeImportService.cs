using HrPanel.Application.Common.Abstractions.LegacyImport;
using HrPanel.Application.Dtos.LegacyImport;
using HrPanel.Domain.Employees;
using HrPanel.Domain.LegacyImport;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace HrPanel.Persistence.LegacyImport;

internal sealed class LegacyEmployeeImportService: ILegacyEmployeeImportService
{
    private readonly HrDbContext _dbContext;
    public LegacyEmployeeImportService(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LegacyEmployeeImportResult> ProcessBatchAsync(Guid batchId,CancellationToken cancellationToken = default)
    {
        if (batchId == Guid.Empty)
        {
            throw new ArgumentException("Batch ID cannot be empty.",nameof(batchId));
        }

        var pendingRowIds = await _dbContext.LegacyEmployeeImportRows
            .AsNoTracking().Where(row =>row.BatchId == batchId && row.ImportStatus == LegacyEmployeeImportStatus.Pending)
            .OrderBy(row => row.SourceRowNumber)
            .Select(row => row.Id)
            .ToListAsync(cancellationToken);

        foreach (var rowId in pendingRowIds)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var claimed = await TryClaimRowAsync(rowId,cancellationToken);

            if (!claimed)
            {
                continue;
            }

            try
            {
               
                await ImportClaimedRowAsync(rowId,CancellationToken.None);
            }
            catch (Exception exception)
            {
                _dbContext.ChangeTracker.Clear();

                await MarkRowAsFailedAsync(rowId,GetErrorDetails(exception),CancellationToken.None);
            }
        }

        return await GetBatchResultAsync(batchId,cancellationToken);
    }

    private async Task<bool> TryClaimRowAsync(long rowId,CancellationToken cancellationToken)
    {
        var affectedRows =await _dbContext.LegacyEmployeeImportRows.Where(row =>row.Id == rowId && row.ImportStatus ==LegacyEmployeeImportStatus.Pending)
                .ExecuteUpdateAsync(updates => updates.SetProperty(
                            row => row.ImportStatus,LegacyEmployeeImportStatus.Processing).SetProperty(row => row.ProcessedAtUtc,(DateTime?)null).SetProperty(row => row.ImportedEmployeeId,(long?)null)
                        .SetProperty(row => row.ErrorDetails,(string?)null),cancellationToken);
                 return affectedRows == 1;
    }

    private async Task ImportClaimedRowAsync(long rowId,CancellationToken cancellationToken)
    {
        var executionStrategy =_dbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            _dbContext.ChangeTracker.Clear();

            await using var transaction =await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var sourceRow = await _dbContext.LegacyEmployeeImportRows.SingleAsync(row =>row.Id == rowId && row.ImportStatus == LegacyEmployeeImportStatus.Processing, cancellationToken);

            var employeeNumber = GetRequiredValue(sourceRow.EmployeeNumber,nameof(sourceRow.EmployeeNumber));

            employeeNumber = NormalizeDigits(employeeNumber)!;

            var legacyUserId = NormalizeOptionalValue(sourceRow.SourceUserId);

            var legacyGuid = ParseOptionalGuid(sourceRow.LegacyId,nameof(sourceRow.LegacyId));

            await EnsureEmployeeDoesNotExistAsync(employeeNumber,legacyUserId,legacyGuid,cancellationToken);

            var employee = Employee.Create(employeeNumber,legacyUserId,legacyGuid);

            AddPersonalDetails(employee, sourceRow);
            AddContacts(employee, sourceRow);
            AddIdentifiers(employee, sourceRow);

            _dbContext.Employees.Add(employee);

            await _dbContext.SaveChangesAsync(cancellationToken);

            sourceRow.MarkAsImported(employee.Id,DateTime.UtcNow);

            //ذخیره دوم، شناسه کارمند تولید شده و وضعیت وارد شده را در ردیف مرحله‌ بندی ذخیره می‌کند هر دو ذخیره هنوز درون یک تراکنش هستند بنابراین با هم ثبت می‌شوند
           
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }
    private async Task EnsureEmployeeDoesNotExistAsync(string employeeNumber,string? legacyUserId,Guid? legacyGuid,CancellationToken cancellationToken)
    {
        var existingEmployee =
            await _dbContext.Employees.AsNoTracking()
                .Where(employee => employee.EmployeeNumber == employeeNumber || (legacyUserId != null && employee.LegacyUserId == legacyUserId) || (legacyGuid.HasValue && employee.LegacyGuid == legacyGuid.Value))
                .Select(employee => new
                {
                    employee.Id,
                    employee.EmployeeNumber,
                    employee.LegacyUserId,
                    employee.LegacyGuid
                })
                .FirstOrDefaultAsync(cancellationToken);

        if (existingEmployee is null)
        {
            return;
        }

        throw new InvalidOperationException($"A normalized employee already exists. " + $"EmployeeId: {existingEmployee.Id}, " + $"EmployeeNumber: {existingEmployee.EmployeeNumber}.");
    }

    private static void AddPersonalDetails(Employee employee,LegacyEmployeeImportRow sourceRow)
    {
        var firstNameFa = GetRequiredValue(sourceRow.FirstNameFa,nameof(sourceRow.FirstNameFa));

        var lastNameFa = GetRequiredValue(sourceRow.LastNameFa,nameof(sourceRow.LastNameFa));

        var personalDetails = EmployeePersonalDetails.Create(firstNameFa,lastNameFa);

        personalDetails.UpdateNames(NormalizeOptionalValue(sourceRow.FirstName),NormalizeOptionalValue(sourceRow.LastName),firstNameFa,lastNameFa);

        personalDetails.UpdatePersonalInformation(birthDate: null,birthPlace: null,gender: ParseGender(sourceRow.Gender),maritalStatus: ParseMaritalStatus(sourceRow.Marital),fatherName:
            NormalizeOptionalValue(sourceRow.FatherName),
            fatherNationalCode:
                NormalizeNationalCode(
                    sourceRow.FatherNationalId));

        employee.SetPersonalDetails(personalDetails);
    }

    private static void AddContacts(Employee employee,LegacyEmployeeImportRow sourceRow)
    {
        AddContactIfPresent(employee,ContactType.Mobile,NormalizeDigits(NormalizeOptionalValue(sourceRow.MobileNumber)),isPrimary: true);

        AddContactIfPresent(employee,ContactType.Telephone,NormalizeDigits(NormalizeOptionalValue(sourceRow.TelephoneNumber)),isPrimary: true);

        AddContactIfPresent(employee,ContactType.AlternateEmail,NormalizeOptionalValue(sourceRow.AlternateEmail),isPrimary: true);
    }

    private static void AddContactIfPresent(Employee employee,ContactType type,string? value,bool isPrimary)
    {
        if (value is null)
        {
            return;
        }

        employee.AddContact(EmployeeContact.Create(type,value,isPrimary));
    }
    private static void AddIdentifiers(Employee employee,LegacyEmployeeImportRow sourceRow)
    {
        AddIdentifierIfPresent(employee,IdentifierType.AccessCard,sourceRow.AccessCard);

        AddIdentifierIfPresent(employee,IdentifierType.ArchiveNumber,sourceRow.ArchiveNumber);

        AddIdentifierIfPresent(employee,IdentifierType.FoodCode,sourceRow.FoodCode);

        AddIdentifierIfPresent(employee,IdentifierType.StaffNumber,sourceRow.StaffNumber);
    }
    private static void AddIdentifierIfPresent(Employee employee,IdentifierType type,string? sourceValue)
    {
        var value = NormalizeOptionalValue(sourceValue);

        if (value is null)
        {
            return;
        }

        employee.AddIdentifier(EmployeeIdentifier.Create(type,NormalizeDigits(value)));
    }
    private async Task MarkRowAsFailedAsync(long rowId,string errorDetails,CancellationToken cancellationToken)
    {
        var sourceRow =
            await _dbContext.LegacyEmployeeImportRows.SingleOrDefaultAsync(row => row.Id == rowId,cancellationToken);

        if (sourceRow is null || sourceRow.ImportStatus !=LegacyEmployeeImportStatus.Processing)
        {
            return;
        }

        sourceRow.MarkAsFailed( errorDetails,DateTime.UtcNow);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<LegacyEmployeeImportResult> GetBatchResultAsync(Guid batchId,CancellationToken cancellationToken)
    {
        _dbContext.ChangeTracker.Clear();

        var counts =
            await _dbContext.LegacyEmployeeImportRows.AsNoTracking()
                .Where(row => row.BatchId == batchId)
                .GroupBy(row => row.ImportStatus)
                .Select(group => new
                {
                    Status = group.Key,
                    Count = group.Count()
                })
                .ToDictionaryAsync(item => item.Status,item => item.Count,cancellationToken);

        int Count(LegacyEmployeeImportStatus status)
        {
            return counts.GetValueOrDefault(status);
        }

        return new LegacyEmployeeImportResult(
            BatchId: batchId,
            TotalRows: counts.Values.Sum(),
            PendingRows:
                Count(LegacyEmployeeImportStatus.Pending),
            ProcessingRows:
                Count(LegacyEmployeeImportStatus.Processing),
            ImportedRows:
                Count(LegacyEmployeeImportStatus.Imported),
            FailedRows:
                Count(LegacyEmployeeImportStatus.Failed),
            SkippedRows:
                Count(LegacyEmployeeImportStatus.Skipped));
    }

    private static string GetRequiredValue(string? value,string fieldName)
    {
        var normalized = NormalizeOptionalValue(value);

        if (normalized is null)
        {
            throw new InvalidOperationException($"Legacy field '{fieldName}' is required.");
        }

        return normalized;
    }

    private static string? NormalizeOptionalValue(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();

        if (normalized.All(character => character == '*'))
        {
            return null;
        }

        if (normalized.Equals("null",StringComparison.OrdinalIgnoreCase) || normalized.Equals("n/a",StringComparison.OrdinalIgnoreCase) || normalized is "-" or "--")
        {
            return null;
        }

        return normalized;
    }

    private static string? NormalizeNationalCode(string? value)
    {
        var normalized = NormalizeOptionalValue(value);

        return normalized is null? null: NormalizeDigits(normalized);
    }

    private static Guid? ParseOptionalGuid(string? value,string fieldName)
    {
        var normalized = NormalizeOptionalValue(value);

        if (normalized is null)
        {
            return null;
        }

        if (Guid.TryParse(normalized, out var result))
        {
            return result;
        }

        throw new InvalidOperationException($"Legacy field '{fieldName}' is not a valid GUID.");
    }

    private static Gender ParseGender(string? value)
    {
        var normalized = NormalizeOptionalValue(value)?.ToLowerInvariant();

        return normalized switch
        {
            "male" or "مرد" => Gender.Male,
            "female" or "زن" => Gender.Female,
            _ => Gender.Unknown
        };
    }

    private static MaritalStatus ParseMaritalStatus(string? value)
    {
        var normalized = NormalizeOptionalValue(value)?.Replace('ي', 'ی').Replace('ك', 'ک').ToLowerInvariant();

        return normalized switch
        {
            "single" or "مجرد" =>MaritalStatus.Single,
            "married" or "متأهل" or "متاهل" =>MaritalStatus.Married,
            "divorced" or "مطلقه" =>MaritalStatus.Divorced,
            "widowed" or "بیوه" =>MaritalStatus.Widowed,_ => MaritalStatus.Unknown
        };
    }

    private static string? NormalizeDigits(string? value)
    {
        if (value is null)
        {
            return null;
        }

        return value
            .Replace('۰', '0')
            .Replace('۱', '1')
            .Replace('۲', '2')
            .Replace('۳', '3')
            .Replace('۴', '4')
            .Replace('۵', '5')
            .Replace('۶', '6')
            .Replace('۷', '7')
            .Replace('۸', '8')
            .Replace('۹', '9')
            .Replace('٠', '0')
            .Replace('١', '1')
            .Replace('٢', '2')
            .Replace('٣', '3')
            .Replace('٤', '4')
            .Replace('٥', '5')
            .Replace('٦', '6')
            .Replace('٧', '7')
            .Replace('٨', '8')
            .Replace('٩', '9');
    }

    private static string GetErrorDetails(
        Exception exception)
    {
        var baseException = exception.GetBaseException();

        return $"{baseException.GetType().Name}: " + baseException.Message;
    }
}

// Execution strategy
//└── Transaction
//    ├── Read row
//    ├── Validate and map
//    ├── Insert employee
//    ├── Mark staging row imported
//    └── Commit

// If a transient SQL problem happens, EF Core can retry that complete unit safely