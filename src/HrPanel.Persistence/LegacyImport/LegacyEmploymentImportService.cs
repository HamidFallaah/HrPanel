using HrPanel.Application.Common.Abstractions.LegacyImport;
using HrPanel.Application.Dtos.LegacyImport;
using HrPanel.Domain.Employment;
using HrPanel.Domain.LegacyImport;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using EmploymentEntity = HrPanel.Domain.Employment.Employment;

namespace HrPanel.Persistence.LegacyImport;

internal sealed class LegacyEmploymentImportService : ILegacyEmploymentImportService
{
    private readonly HrDbContext _dbContext;

    public LegacyEmploymentImportService(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LegacyEmploymentImportResult> ImportAsync(Guid batchId,CancellationToken cancellationToken = default)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            _dbContext.ChangeTracker.Clear();

            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable,cancellationToken);

            var rows = await _dbContext.LegacyEmployeeImportRows
                .AsNoTracking()
                .Where(row =>
                    row.BatchId == batchId &&
                    row.ImportStatus ==
                    LegacyEmployeeImportStatus.Imported)
                .OrderBy(row => row.SourceRowNumber)
                .ToListAsync(cancellationToken);

            if (rows.Count == 0)
            {
                throw new InvalidOperationException($"No imported staging rows were found " + $"for batch '{batchId}'.");
            }

            var employeeIds = rows
                .Where(row => row.ImportedEmployeeId.HasValue)
                .Select(row => row.ImportedEmployeeId!.Value)
                .Distinct()
                .ToArray();

            var employmentsByEmployeeId =
                (await _dbContext.Employments
                    .Include(employment =>
                        employment.Assignments)
                    .Where(employment =>
                        employeeIds.Contains(
                            employment.EmployeeId))
                    .OrderByDescending(employment =>
                        employment.StartDate)
                    .ToListAsync(cancellationToken))
                .GroupBy(employment =>
                    employment.EmployeeId)
                .ToDictionary(
                    group => group.Key,
                    group => group.First());

            var employmentTypes =
                await _dbContext.EmploymentTypes
                    .AsNoTracking()
                    .ToDictionaryAsync(
                        type => type.Code,
                        StringComparer.OrdinalIgnoreCase,
                        cancellationToken);

            var employmentStatuses =
                await _dbContext.EmploymentStatuses
                    .AsNoTracking()
                    .ToDictionaryAsync(
                        status => status.Code,
                        StringComparer.OrdinalIgnoreCase,
                        cancellationToken);

            var workTimeTypes =
                await _dbContext.WorkTimeTypes
                    .AsNoTracking()
                    .ToDictionaryAsync(
                        type => type.Code,
                        StringComparer.OrdinalIgnoreCase,
                        cancellationToken);

            var organizationUnitIds =
                await _dbContext.OrganizationUnits
                    .AsNoTracking()
                    .ToDictionaryAsync(
                        unit => unit.Code,
                        unit => unit.Id,
                        StringComparer.OrdinalIgnoreCase,
                        cancellationToken);

            var positionIds =
                await _dbContext.Positions
                    .AsNoTracking()
                    .ToDictionaryAsync(
                        position => position.Code,
                        position => position.Id,
                        StringComparer.OrdinalIgnoreCase,
                        cancellationToken);

            var jobLevelIds =
                await _dbContext.JobLevels
                    .AsNoTracking()
                    .ToDictionaryAsync(
                        level => level.Code,
                        level => level.Id,
                        StringComparer.OrdinalIgnoreCase,
                        cancellationToken);

            var workLocationIds =
                await _dbContext.WorkLocations
                    .AsNoTracking()
                    .ToDictionaryAsync(
                        location => location.Code,
                        location => location.Id,
                        StringComparer.OrdinalIgnoreCase,
                        cancellationToken);

            EnsureRequiredLookups(employmentTypes,employmentStatuses,workTimeTypes);

            var counters = new ImportCounters();
            var warnings = new List<string>();

            foreach (var row in rows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!row.ImportedEmployeeId.HasValue)
                {
                    Skip(row,"ImportedEmployeeId is missing.",counters,warnings);
                    continue;
                }

                var startDateSource = FirstValue(
                    row.StartWorkFirst,
                    row.StartWork);

                if (!TryParseDate(
                        startDateSource,
                        out var employmentStartDate))
                {
                    Skip(row,"Employment start date is missing or invalid.",counters,warnings);
                    continue;
                }

                var assignmentStartDate = TryParseDate(row.StartWork,out var parsedAssignmentStartDate)? parsedAssignmentStartDate: employmentStartDate;

                if (assignmentStartDate < employmentStartDate)
                {
                    assignmentStartDate = employmentStartDate;
                }

                DateOnly? employmentEndDate = null;

                if (Clean(row.EndWork) is not null)
                {
                    if (!TryParseDate(row.EndWork,out var parsedEndDate))
                    {
                        Skip(row,"Employment end date is invalid.",counters,warnings);
                        continue;
                    }

                    if (parsedEndDate < employmentStartDate)
                    {
                        Skip(row,$"Employment end date " + $"'{parsedEndDate:yyyy-MM-dd}' is before " +$"start date " + $"'{employmentStartDate:yyyy-MM-dd}'.",counters,warnings);
                        continue;
                    }

                    employmentEndDate = parsedEndDate;
                }

                var employmentTypeCode = MapEmploymentTypeCode(row.EmploymentType);

                if (employmentTypeCode is null ||!employmentTypes.TryGetValue(employmentTypeCode,out var employmentType))
                {
                    Skip(row,$"Employment type " +$"'{Clean(row.EmploymentType) ?? "<empty>"}' " +"is unresolved.",counters,warnings);
                    continue;
                }

                var employmentStatusCode = MapEmploymentStatusCode(row.EmploymentStatus);

                if (employmentStatusCode is null || !employmentStatuses.TryGetValue(employmentStatusCode, out var employmentStatus))
                {
                    Skip(row,$"Employment status " + $"'{Clean(row.EmploymentStatus) ?? "<empty>"}' " +"is unresolved.",counters,warnings);

                    continue;
                }

                var contractTermMonths =
                    ParseContractTermMonths(row,warnings);

                var workTimeTypeId = ResolveWorkTimeTypeId(row,workTimeTypes,warnings);

                var employeeId = row.ImportedEmployeeId.Value;

                if (!employmentsByEmployeeId.TryGetValue(
                        employeeId,
                        out var employment))
                {
                    employment = EmploymentEntity.Start(
                        employeeId,
                        employmentType.Id,
                        employmentStatus.Id,
                        employmentStartDate,
                        contractTermMonths,
                        workTimeTypeId);

                    AddAssignmentIfPresent(
                        employment,
                        row,
                        AssignmentContext.Hr,
                        assignmentStartDate,
                        organizationUnitIds,
                        positionIds,
                        jobLevelIds,
                        workLocationIds,
                        counters,
                        warnings);

                    AddAssignmentIfPresent(
                        employment,
                        row,
                        AssignmentContext.Cr,
                        assignmentStartDate,
                        organizationUnitIds,
                        positionIds,
                        jobLevelIds,
                        workLocationIds,
                        counters,
                        warnings);

                    if (employmentEndDate.HasValue)
                    {
                        employment.End(
                        employmentEndDate.Value,
                        employmentStatus.Id,
                        reason: null);
                    }

                    _dbContext.Employments.Add(employment);

                    employmentsByEmployeeId.Add(
                        employeeId,
                        employment);

                    counters.EmploymentsCreated++;

                    continue;
                }

                counters.EmploymentsReused++;

                BackfillWorkTimeType(
                    employment,
                    workTimeTypeId,
                    row,
                    warnings);

                WarnAboutEmploymentMismatch(
                    employment,
                    row,
                    employmentType.Id,
                    employmentStatus.Id,
                    employmentStartDate,
                    employmentEndDate,
                    warnings);

                ReuseOrAddAssignment(
                    employment,
                    row,
                    AssignmentContext.Hr,
                    assignmentStartDate,
                    organizationUnitIds,
                    positionIds,
                    jobLevelIds,
                    workLocationIds,
                    counters,
                    warnings);

                ReuseOrAddAssignment(
                    employment,
                    row,
                    AssignmentContext.Cr,
                    assignmentStartDate,
                    organizationUnitIds,
                    positionIds,
                    jobLevelIds,
                    workLocationIds,
                    counters,
                    warnings);
            }

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new LegacyEmploymentImportResult(
                batchId,
                rows.Count,
                counters.EmploymentsCreated,
                counters.EmploymentsReused,
                counters.HrAssignmentsCreated,
                counters.HrAssignmentsReused,
                counters.CrAssignmentsCreated,
                counters.CrAssignmentsReused,
                counters.RowsSkipped,
                warnings);
        });
    }

    private static void ReuseOrAddAssignment(
    EmploymentEntity employment,
    LegacyEmployeeImportRow row,
    AssignmentContext context,
    DateOnly effectiveFrom,
    IReadOnlyDictionary<string, long> organizationUnitIds,
    IReadOnlyDictionary<string, long> positionIds,
    IReadOnlyDictionary<string, short> jobLevelIds,
    IReadOnlyDictionary<string, long> workLocationIds,
    ImportCounters counters,
    ICollection<string> warnings)
    {
        var existingAssignment = employment.Assignments.FirstOrDefault(assignment => assignment.Context == context);

        if (existingAssignment is not null)
        {
            IncrementReused(context, counters);
            return;
        }

        var hasSourceData =
            context == AssignmentContext.Hr ? HasHrAssignmentData(row) : HasCrAssignmentData(row);

        if (!hasSourceData)
        {
            return;
        }

        if (!employment.IsCurrent)
        {
            warnings.Add($"Row {row.SourceRowNumber}: cannot add a missing " + $"{context} assignment to an ended employment.");

            return;
        }

        AddAssignmentIfPresent(
            employment,
            row,
            context,
            effectiveFrom,
            organizationUnitIds,
            positionIds,
            jobLevelIds,
            workLocationIds,
            counters,
            warnings);
    }

    private static void AddAssignmentIfPresent(
        EmploymentEntity employment,
        LegacyEmployeeImportRow row,
        AssignmentContext context,
        DateOnly effectiveFrom,
        IReadOnlyDictionary<string, long> organizationUnitIds,
        IReadOnlyDictionary<string, long> positionIds,
        IReadOnlyDictionary<string, short> jobLevelIds,
        IReadOnlyDictionary<string, long> workLocationIds,
        ImportCounters counters,
        ICollection<string> warnings)
    {
        var hasSourceData = context == AssignmentContext.Hr ? HasHrAssignmentData(row) : HasCrAssignmentData(row);

        if (!hasSourceData)
        {
            return;
        }

        var organizationUnitId =
            ResolveOrganizationUnitId(row, context, organizationUnitIds, warnings);

        var positionName = context == AssignmentContext.Hr ? Clean(row.PositionHr) : Clean(row.PositionCr);

        var positionId = ResolveStableReferenceId(
            positionName,
            "POS",
            positionIds,
            row,
            $"{context} position",
            warnings);

        var jobLevelId = ResolveJobLevelId(
            row,
            jobLevelIds,
            warnings);

        var workLocationId = ResolveWorkLocationId(
            row,
            workLocationIds,
            warnings);

        var assignment = EmployeeAssignment.Create(
            context,
            effectiveFrom,
            organizationUnitId,
            positionId,
            jobLevelId,
            workLocationId);

        employment.AddAssignment(assignment);

        IncrementCreated(
            context,
            counters);
    }

    private static long? ResolveOrganizationUnitId(
        LegacyEmployeeImportRow row,
        AssignmentContext context,
        IReadOnlyDictionary<string, long> organizationUnitIds,
        ICollection<string> warnings)
    {
        var tree =
            context == AssignmentContext.Hr
                ? "HR"
                : "CR";

        var path = new List<string>
        {
            tree
        };

        var hierarchyParts =
            context == AssignmentContext.Hr
                ? new[]
                {
                    new HierarchyPart(
                        "DIVISION",
                        row.Division),

                    new HierarchyPart(
                        "SUBDIVISION",
                        row.SubDivision),

                    new HierarchyPart(
                        "DEPARTMENT",
                        row.Department),

                    new HierarchyPart(
                        "SECTION",
                        row.Section)
                }
                : new[]
                {
                    new HierarchyPart(
                        "DIVISION",
                        row.DivisionCr)
                };

        string? resolvedCode = null;

        foreach (var hierarchyPart in hierarchyParts)
        {
            var name = Clean(hierarchyPart.Name);

            if (name is null)
            {
                continue;
            }

            path.Add(
                $"{hierarchyPart.TypeCode}:{Normalize(name)}");

            resolvedCode = CreateStableCode(
                $"OU-{tree}",
                string.Join("/", path));
        }

        if (resolvedCode is null)
        {
            return null;
        }

        if (organizationUnitIds.TryGetValue(
                resolvedCode,
                out var organizationUnitId))
        {
            return organizationUnitId;
        }

        warnings.Add($"Row {row.SourceRowNumber}: {context} organization " + $"unit '{resolvedCode}' was not found. " +"Run the organization import first.");

        return null;
    }

    private static long? ResolveWorkLocationId(
        LegacyEmployeeImportRow row,
        IReadOnlyDictionary<string, long> workLocationIds,
        ICollection<string> warnings)
    {
        var name = Clean(row.WorkLocation);
        var province = Clean(row.ProvinceWork);
        var city = Clean(row.CityWork);

        if (name is null &&
            province is null &&
            city is null)
        {
            return null;
        }

        var identity =
            $"{Normalize(name)}|" +
            $"{Normalize(province)}|" +
            $"{Normalize(city)}";

        var code = CreateStableCode(
            "LOC",
            identity);

        if (workLocationIds.TryGetValue(
                code,
                out var workLocationId))
        {
            return workLocationId;
        }

        warnings.Add(
            $"Row {row.SourceRowNumber}: work location " + $"'{code}' was not found. " + "Run the organization import first.");

        return null;
    }

    private static long? ResolveStableReferenceId(string? value,string prefix,IReadOnlyDictionary<string, long> ids,LegacyEmployeeImportRow row,string fieldName,ICollection<string> warnings)
    {
        if (value is null)
        {
            return null;
        }

        var code = CreateStableCode(
            prefix,
            Normalize(value));

        if (ids.TryGetValue(code, out var id))
        {
            return id;
        }

        warnings.Add(
            $"Row {row.SourceRowNumber}: " +
            $"{fieldName} '{value}' was not found.");

        return null;
    }

    private static short? ResolveJobLevelId(LegacyEmployeeImportRow row,IReadOnlyDictionary<string, short> jobLevelIds,ICollection<string> warnings)
    {
        var code = NormalizeDigits(
            Clean(row.JobLevel));

        if (code is null)
        {
            return null;
        }

        // Legacy level 0 belongs to company level 1
        if (code == "0")
        {
            code = "1";
        }

        if (jobLevelIds.TryGetValue(
                code,
                out var jobLevelId))
        {
            return jobLevelId;
        }

        warnings.Add(
            $"Row {row.SourceRowNumber}: " +
            $"job level '{code}' was not found.");

        return null;
    }

    private static string? MapEmploymentTypeCode(string? value)
    {
        return Normalize(value) switch
        {
            "VENDOR" => "VENDOR",
            "LOCAL" => "LOCAL",
            "PERMANENT" => "PERMANENT",
            "CONTRACT" => "CONTRACT",
            "PROJECT" => "PROJECT",
            _ => null
        };
    }

    private static string? MapEmploymentStatusCode(string? value)
    {
        return Normalize(value) switch
        {
            "ACTIVE" => "ACTIVE",
            "INACTIVE" => "INACTIVE",

            "TERMINATE" or "TERMINATED"
                => "TERMINATED",

            "RESIGN" or "RESIGNED"
                => "RESIGNED",

            "MATERNITY LEAVE"
                => "MATERNITY_LEAVE",

            "TRANSFER" or "TRANSFERRED"
                => "TRANSFERRED",

            _ => null
        };
    }

    private static short? ResolveWorkTimeTypeId(LegacyEmployeeImportRow row,IReadOnlyDictionary<string, WorkTimeType> workTimeTypes,ICollection<string> warnings)
    {
        var activityType = Clean(row.ActivityType);

        if (activityType is null)
        {
            return null;
        }

        var workTimeTypeCode =
            MapWorkTimeTypeCode(activityType);

        if (workTimeTypeCode is null)
        {
            warnings.Add(
                $"Row {row.SourceRowNumber}: activity type " + $"'{activityType}' is unknown; WorkTimeTypeId " + "was left empty.");

            return null;
        }

        return workTimeTypes[workTimeTypeCode].Id;
    }

    private static string? MapWorkTimeTypeCode(
        string? value)
    {
        var normalized = Normalize(value).Replace('_', ' ').Replace('-', ' ');

        normalized = string.Join(' ',normalized.Split(' ',StringSplitOptions.RemoveEmptyEntries));

        return normalized switch
        {
            "FULL TIME" or "FULLTIME"=> "FULL_TIME",

            "PART TIME" or "PARTTIME"=> "PART_TIME", _ => null
        };
    }

    private void BackfillWorkTimeType(EmploymentEntity employment,short? importedWorkTimeTypeId,LegacyEmployeeImportRow row,ICollection<string> warnings)
    {
        if (!importedWorkTimeTypeId.HasValue)
        {
            return;
        }

        if (employment.WorkTimeTypeId ==
            importedWorkTimeTypeId.Value)
        {
            return;
        }

        if (employment.WorkTimeTypeId.HasValue)
        {
            warnings.Add(
                $"Row {row.SourceRowNumber}: employment for " +
                $"employee {row.ImportedEmployeeId} already has " +
                $"WorkTimeTypeId {employment.WorkTimeTypeId.Value}, " +
                $"but the imported activity type resolves to " +
                $"{importedWorkTimeTypeId.Value}. The existing value " +
                "was preserved.");

            return;
        }

        // This is an intentional persistence-level backfill. It also updates
        // ended employments, whose normal domain mutation methods reject edits.
        
        var property = _dbContext.Entry(employment).Property(entity => entity.WorkTimeTypeId);

        property.CurrentValue =importedWorkTimeTypeId.Value;

        property.IsModified = true;
    }

    private static void EnsureRequiredLookups(
        IReadOnlyDictionary<string, EmploymentType> employmentTypes,
        IReadOnlyDictionary<string, EmploymentStatus> employmentStatuses,
        IReadOnlyDictionary<string, WorkTimeType> workTimeTypes)
    {
        var requiredEmploymentTypes = new[]
        {
            "VENDOR",
            "LOCAL"
        };

        var requiredEmploymentStatuses = new[]
        {
            "ACTIVE",
            "TERMINATED",
            "RESIGNED",
            "MATERNITY_LEAVE",
            "TRANSFERRED"
        };

        var requiredWorkTimeTypes = new[]
        {
            "FULL_TIME",
            "PART_TIME"
        };

        var missingTypes = requiredEmploymentTypes
            .Where(code =>
                !employmentTypes.ContainsKey(code));

        var missingStatuses = requiredEmploymentStatuses
            .Where(code =>
                !employmentStatuses.ContainsKey(code));

        var missingWorkTimeTypes = requiredWorkTimeTypes
            .Where(code =>
                !workTimeTypes.ContainsKey(code));

        var missingCodes = missingTypes
            .Concat(missingStatuses)
            .Concat(missingWorkTimeTypes)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (missingCodes.Length > 0)
        {
            throw new InvalidOperationException(
                $"Missing employment lookup codes: " +
                $"{string.Join(", ", missingCodes)}.");
        }
    }

    private static short? ParseContractTermMonths(LegacyEmployeeImportRow row,ICollection<string> warnings)
    {
        var value = NormalizeDigits(
            Clean(row.ContractTerm));

        if (value is null)
        {
            return null;
        }

        if (short.TryParse(
                value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var months) &&
            months is >= 1 and <= 120)
        {
            return months;
        }

        warnings.Add($"Row {row.SourceRowNumber}: contract term " + $"'{value}' was ignored; expected an integer " +"from 1 to 120 months.");

        return null;
    }

    private static bool TryParseDate(string? value,out DateOnly date)
    {
        var cleaned = NormalizeDigits(
            Clean(value));

        if (cleaned is not null && DateTime.TryParse(cleaned,CultureInfo.GetCultureInfo("en-US"),DateTimeStyles.AllowWhiteSpaces,out var parsedDateTime))
        {
            date = DateOnly.FromDateTime(parsedDateTime);

            return true;
        }

        date = default;
        return false;
    }

    private static void WarnAboutEmploymentMismatch(
        EmploymentEntity employment,
        LegacyEmployeeImportRow row,
        short employmentTypeId,
        short employmentStatusId,
        DateOnly startDate,
        DateOnly? endDate,
        ICollection<string> warnings)
    {
        var valuesMatch =
            employment.EmploymentTypeId == employmentTypeId &&
            employment.EmploymentStatusId == employmentStatusId &&
            employment.StartDate == startDate &&
            employment.EndDate == endDate;

        if (valuesMatch)
        {
            return;
        }

        warnings.Add(
            $"Row {row.SourceRowNumber}: an employment already " +
            $"exists for employee {row.ImportedEmployeeId}, but " +
            "one or more imported values differ. The existing " +
            "normalized employment was preserved.");
    }

    private static void IncrementCreated(AssignmentContext context,ImportCounters counters)
    {
        if (context == AssignmentContext.Hr)
        {
            counters.HrAssignmentsCreated++;
        }
        else
        {
            counters.CrAssignmentsCreated++;
        }
    }

    private static void IncrementReused(AssignmentContext context,ImportCounters counters)
    {
        if (context == AssignmentContext.Hr)
        {
            counters.HrAssignmentsReused++;
        }
        else
        {
            counters.CrAssignmentsReused++;
        }
    }

    private static bool HasHrAssignmentData(LegacyEmployeeImportRow row)
    {
        return new[]
        {
            row.Division,
            row.SubDivision,
            row.Department,
            row.Section,
            row.PositionHr,
            row.JobLevel,
            row.WorkLocation,
            row.ProvinceWork,
            row.CityWork
        }.Any(value => Clean(value) is not null);
    }

    private static bool HasCrAssignmentData(LegacyEmployeeImportRow row)
    {
        return new[]
        {
            row.DivisionCr,
            row.PositionCr
        }.Any(value => Clean(value) is not null);
    }

    private static void Skip(LegacyEmployeeImportRow row,string reason,ImportCounters counters,ICollection<string> warnings)
    {
        counters.RowsSkipped++;

        warnings.Add( $"Row {row.SourceRowNumber}: {reason}");
    }

    private static string? FirstValue(params string?[] values)
    {
        return values.Select(Clean).FirstOrDefault(value => value is not null);
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null: value.Trim();
    }

    private static string Normalize(string? value)
    {
        return Clean(value)?.ToUpperInvariant()?? string.Empty;
    }

    private static string? NormalizeDigits(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var characters = value.Select(character =>
            character switch
            {
                '۰' => '0',
                '۱' => '1',
                '۲' => '2',
                '۳' => '3',
                '۴' => '4',
                '۵' => '5',
                '۶' => '6',
                '۷' => '7',
                '۸' => '8',
                '۹' => '9',

                '٠' => '0',
                '١' => '1',
                '٢' => '2',
                '٣' => '3',
                '٤' => '4',
                '٥' => '5',
                '٦' => '6',
                '٧' => '7',
                '٨' => '8',
                '٩' => '9',

                _ => character
            });

        return new string(characters.ToArray());
    }

    private static string CreateStableCode(string prefix,string identity)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(identity));

        return $"{prefix}-" + $"{Convert.ToHexString(hash)[..20]}";
    }

    private sealed record HierarchyPart(string TypeCode,string? Name);

    private sealed class ImportCounters
    {
        public int EmploymentsCreated { get; set; }
        public int EmploymentsReused { get; set; }
        public int HrAssignmentsCreated { get; set; }
        public int HrAssignmentsReused { get; set; }
        public int CrAssignmentsCreated { get; set; }
        public int CrAssignmentsReused { get; set; }
        public int RowsSkipped { get; set; }
    }
}