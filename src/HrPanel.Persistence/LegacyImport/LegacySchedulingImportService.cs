using System.Data;
using System.Globalization;
using HrPanel.Application.Common.Abstractions.LegacyImport;
using HrPanel.Application.Dtos.LegacyImport;
using HrPanel.Domain.LegacyImport;
using HrPanel.Domain.Scheduling;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using EmploymentEntity = HrPanel.Domain.Employment.Employment;

namespace HrPanel.Persistence.LegacyImport;

internal sealed class LegacySchedulingImportService : ILegacySchedulingImportService
{
    private const string OfficeLongShiftCode = "OFFICE_LONG";
    private const string OfficeShortShiftCode = "OFFICE_SHORT";
    private const string OfficeScheduleCode = "OFFICE_WEEKLY";

    private static readonly TimeOnly OfficeStartTime = new(8, 0);
    private static readonly TimeOnly OfficeLongEndTime = new(17, 15);
    private static readonly TimeOnly OfficeShortEndTime = new(15, 0);
    private readonly HrDbContext _dbContext;

    public LegacySchedulingImportService(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LegacySchedulingImportResult> ImportAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        if (batchId == Guid.Empty)
        {
            throw new ArgumentException("Batch ID cannot be empty.", nameof(batchId));
        }

        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            _dbContext.ChangeTracker.Clear();

            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    cancellationToken);

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

            var counters = new ImportCounters();
            var warnings = new List<string>();

            var officeRows = rows.Where(row => MapsToOfficeSchedule(row, counters, warnings)).ToList();

            if (officeRows.Count == 0)
            {
                throw new InvalidOperationException("No confirmed ShiftType='Office_Time' rows " + "were found in this batch.");
            }

            var officeLongShift =
                await GetOrCreateShiftAsync(
                    OfficeLongShiftCode,
                    "اداری شنبه تا سه‌شنبه",
                    "Office Saturday-Tuesday",
                    OfficeStartTime,
                    OfficeLongEndTime,
                    9.25m,
                    counters,
                    cancellationToken);

            var officeShortShift =
                await GetOrCreateShiftAsync(
                    OfficeShortShiftCode,
                    "اداری چهارشنبه",
                    "Office Wednesday",
                    OfficeStartTime,
                    OfficeShortEndTime,
                    7m,
                    counters,
                    cancellationToken);

            // IDs are required by WorkScheduleDay
            await _dbContext.SaveChangesAsync(cancellationToken);

            var officeSchedule = await GetOrCreateOfficeScheduleAsync(officeLongShift.Id, officeShortShift.Id, counters, cancellationToken);

            // WorkSchedule ID is required by schedule assignments
            await _dbContext.SaveChangesAsync(cancellationToken);

            var employeeIds = officeRows
                .Where(row => row.ImportedEmployeeId.HasValue)
                .Select(row => row.ImportedEmployeeId!.Value)
                .Distinct()
                .ToArray();

            var employmentsByEmployeeId =
                (await _dbContext.Employments
                    .AsNoTracking()
                    .Where(employment => employeeIds.Contains(employment.EmployeeId))
                    .OrderByDescending(employment => employment.StartDate)
                    .ToListAsync(cancellationToken))
                    .GroupBy(employment => employment.EmployeeId)
                    .ToDictionary(
                        group => group.Key,
                        group => group.First());

            var employmentIds = employmentsByEmployeeId
                .Values
                .Select(employment => employment.Id)
                .ToArray();

            var assignmentsByEmploymentId =
                (await _dbContext.EmployeeScheduleAssignments
                    .AsNoTracking()
                    .Where(assignment =>
                        employmentIds.Contains(
                            assignment.EmploymentId))
                    .ToListAsync(cancellationToken))
                .GroupBy(assignment =>
                    assignment.EmploymentId)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToList());

            foreach (var row in officeRows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!row.ImportedEmployeeId.HasValue)
                {
                    Skip(row, "ImportedEmployeeId is missing.", counters, warnings);
                    continue;
                }

                if (!employmentsByEmployeeId.TryGetValue(row.ImportedEmployeeId.Value, out var employment))
                {
                    Skip(row, "No current normalized employment was found.", counters, warnings);
                    continue;
                }


                var effectiveFrom =
    ResolveEffectiveFrom(row, employment, warnings);

                var effectiveTo = employment.EndDate;

                if (TryReuseAssignment(
                        employment.Id,
                        officeSchedule.Id,
                        effectiveFrom,
                        effectiveTo,
                        assignmentsByEmploymentId,
                        counters,
                        row,
                        warnings))
                {
                    continue;
                }

                var assignment =
                    EmployeeScheduleAssignment.Create(
                        employment.Id,
                        officeSchedule.Id,
                        effectiveFrom,
                        rotationOffsetDays: 0);

                if (effectiveTo.HasValue)
                {
                    assignment.End(effectiveTo.Value);
                }

                _dbContext.EmployeeScheduleAssignments.Add(assignment);

                if (!assignmentsByEmploymentId.TryGetValue(employment.Id, out var employmentAssignments))
                {
                    employmentAssignments = [];

                    assignmentsByEmploymentId.Add(employment.Id, employmentAssignments);
                }

                employmentAssignments.Add(assignment);

                counters.AssignmentsCreated++;
                counters.RowsAssigned++;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new LegacySchedulingImportResult(
                batchId,
                rows.Count,
                counters.ShiftsCreated,
                counters.ShiftsReused,
                counters.WorkSchedulesCreated,
                counters.WorkSchedulesReused,
                counters.AssignmentsCreated,
                counters.AssignmentsReused,
                counters.RowsAssigned,
                counters.RowsWithoutScheduleData,
                counters.RowsUnresolved,
                counters.RowsSkipped,
                warnings);
        });
    }

    private async Task<Shift> GetOrCreateShiftAsync(string code, string nameFa, string nameEn, TimeOnly startTime, TimeOnly endTime, decimal workHours, ImportCounters counters, CancellationToken cancellationToken)
    {
        var shift = await _dbContext.Shifts
            .SingleOrDefaultAsync(
                existingShift => existingShift.Code == code,
                cancellationToken);

        if (shift is null)
        {
            shift = Shift.Create(code, nameFa, nameEn, startTime, endTime, workHours);

            _dbContext.Shifts.Add(shift);
            counters.ShiftsCreated++;

            return shift;
        }

        if (shift.StartTime != startTime ||
            shift.EndTime != endTime ||
            shift.WorkHours != workHours)
        {
            throw new InvalidOperationException($"Shift '{code}' already exists with different " + "times or work hours. Import stopped to protect " + "the existing reference data");
        }

        if (!shift.IsActive)
        {
            throw new InvalidOperationException($"Shift '{code}' exists but is inactive");
        }

        counters.ShiftsReused++;

        return shift;
    }

    private async Task<WorkSchedule> GetOrCreateOfficeScheduleAsync(long officeLongShiftId, long officeShortShiftId, ImportCounters counters, CancellationToken cancellationToken)
    {
        var schedule = await _dbContext.WorkSchedules
            .Include(existingSchedule => existingSchedule.Days)
            .SingleOrDefaultAsync(
                existingSchedule =>
                    existingSchedule.Code == OfficeScheduleCode,
                cancellationToken);

        if (schedule is not null)
        {
            ValidateOfficeSchedule(
                schedule,
                officeLongShiftId,
                officeShortShiftId);

            counters.WorkSchedulesReused++;

            return schedule;
        }

        schedule = WorkSchedule.Create(
            OfficeScheduleCode,
            "برنامه هفتگی اداری",
            "Office Weekly",
            WorkSchedulePatternType.Weekly,
            cycleLengthDays: 7);

        // 0 = Saturday
        schedule.AddWorkingDay(0, officeLongShiftId);

        // 1 = Sunday
        schedule.AddWorkingDay(1, officeLongShiftId);

        // 2 = Monday
        schedule.AddWorkingDay(2, officeLongShiftId);

        // 3 = Tuesday
        schedule.AddWorkingDay(3, officeLongShiftId);

        // 4 = Wednesday
        schedule.AddWorkingDay(4, officeShortShiftId);

        // 5 = Thursday
        schedule.AddRestDay(5);

        // 6 = Friday
        schedule.AddRestDay(6);

        _dbContext.WorkSchedules.Add(schedule);
        counters.WorkSchedulesCreated++;

        return schedule;
    }

    private static void ValidateOfficeSchedule(WorkSchedule schedule, long officeLongShiftId, long officeShortShiftId)
    {
        var expectedDays =
            new Dictionary<short, long?>
            {
                [0] = officeLongShiftId,
                [1] = officeLongShiftId,
                [2] = officeLongShiftId,
                [3] = officeLongShiftId,
                [4] = officeShortShiftId,
                [5] = null,
                [6] = null
            };

        var daysMatch =
            schedule.Days.Count == expectedDays.Count &&
            schedule.Days.All(day =>
                expectedDays.TryGetValue(
                    day.DayIndex,
                    out var expectedShiftId) &&
                day.ShiftId == expectedShiftId &&
                day.IsRestDay ==
                    !expectedShiftId.HasValue);

        if (!schedule.IsActive || schedule.PatternType != WorkSchedulePatternType.Weekly || schedule.CycleLengthDays != 7 || schedule.AnchorDate.HasValue || !daysMatch)
        {
            throw new InvalidOperationException($"Work schedule '{OfficeScheduleCode}' already " + "exists with a different pattern. Import stopped " + "to protect the existing schedule.");
        }
    }

    private static bool MapsToOfficeSchedule(LegacyEmployeeImportRow row, ImportCounters counters, ICollection<string> warnings)
    {
        var shiftType = NormalizeScheduleValue(row.ShiftType);

        if (shiftType == "OFFICE TIME")
        {
            return true;
        }

        if (shiftType == "SHIFT NORMAL")
        {
            Unresolved(row, "SHIFT_TYPE 'Shift_Normal' requires exact shift times, " + "rotation sequence, and employee rotation offset.", counters, warnings);
            return false;
        }

        if (shiftType == "RESIGN")
        {
            Unresolved(row, "SHIFT_TYPE 'Resign' is an employment status, " + "not a work schedule.", counters, warnings);
            return false;
        }

        if (shiftType is not null)
        {
            Unresolved(row, $"SHIFT_TYPE '{Clean(row.ShiftType)}' is unknown.", counters, warnings);
            return false;
        }

        var unit = Clean(row.Unit);

        if (unit is "06:00-15:00" or "06:00-13:15")
        {
            Unresolved(row, $"Unit contains time range '{unit}', but its working " + "weekdays and effective period are unknown.", counters, warnings);

            return false;
        }

        counters.RowsWithoutScheduleData++;

        return false;
    }

    private static bool IsOfficeTime(string? value)
    {
        return string.Equals(value?.Trim(), "Office_Time", StringComparison.OrdinalIgnoreCase);
    }

    private static DateOnly ResolveEffectiveFrom(LegacyEmployeeImportRow row, EmploymentEntity employment, ICollection<string> warnings)
    {
        if (!TryParseDate(row.StartWork, out var effectiveFrom))
        {
            return employment.StartDate;
        }

        if (effectiveFrom < employment.StartDate)
        {
            warnings.Add(
                $"Row {row.SourceRowNumber}: schedule start date " +
                $"{effectiveFrom:yyyy-MM-dd} is before employment " +
                $"start date {employment.StartDate:yyyy-MM-dd}; " +
                "employment start date was used.");

            return employment.StartDate;
        }

        return effectiveFrom;
    }

    private static bool TryReuseAssignment(
     long employmentId,
     long workScheduleId,
     DateOnly effectiveFrom,
     DateOnly? effectiveTo,
     IReadOnlyDictionary<long, List<EmployeeScheduleAssignment>> assignmentsByEmploymentId,
     ImportCounters counters,
     LegacyEmployeeImportRow row,
     ICollection<string> warnings) 
    {
        {
            if (!assignmentsByEmploymentId.TryGetValue(employmentId, out var assignments))
            {
                return false;
            }

            var exactAssignment = assignments.FirstOrDefault(
                assignment =>
                    assignment.WorkScheduleId == workScheduleId &&
                    assignment.EffectiveFrom == effectiveFrom &&
                    assignment.EffectiveTo == effectiveTo &&
                    assignment.RotationOffsetDays == 0);

            if (exactAssignment is not null)
            {
                counters.AssignmentsReused++;
                counters.RowsAssigned++;

                return true;
            }

            var overlappingAssignment = assignments.FirstOrDefault(assignment => DateRangesOverlap(assignment.EffectiveFrom, assignment.EffectiveTo, effectiveFrom, effectiveTo));

            if (overlappingAssignment is null)
            {
                return false;
            }

            Unresolved(row,$"An overlapping schedule assignment already exists " +$"for employment {employmentId}; it was preserved.",counters, warnings);

            return true;
        }
    }
    private static bool DateRangesOverlap(DateOnly firstFrom, DateOnly? firstTo, DateOnly secondFrom, DateOnly? secondTo)
    {
        return (!firstTo.HasValue || secondFrom <= firstTo.Value) && (!secondTo.HasValue || firstFrom <= secondTo.Value);
    }

    private static bool TryParseDate(string? value, out DateOnly date)
    {
        var cleaned = NormalizeDigits(Clean(value));

        if (cleaned is not null && DateTime.TryParse(cleaned, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AllowWhiteSpaces, out var parsedDateTime))
        {
            date = DateOnly.FromDateTime(parsedDateTime);

            return true;
        }

        date = default;

        return false;
    }
    private static string? NormalizeScheduleValue(string? value)
    {
        var cleaned = Clean(value);

        if (cleaned is null)
        {
            return null;
        }

        var normalized = cleaned.Replace('_', ' ').Replace('-', ' ').ToUpperInvariant();

        return string.Join(' ', normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private static string? Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var cleaned = value.Trim();

        return cleaned.Equals("NULL", StringComparison.OrdinalIgnoreCase) ? null : cleaned;
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

    private static void Skip(LegacyEmployeeImportRow row, string reason, ImportCounters counters, ICollection<string> warnings)
    {
        counters.RowsSkipped++;

        warnings.Add($"Row {row.SourceRowNumber}: {reason}");
    }

    private static void Unresolved(LegacyEmployeeImportRow row, string reason, ImportCounters counters, ICollection<string> warnings)
    {
        counters.RowsUnresolved++;

        warnings.Add($"Row {row.SourceRowNumber}: {reason}");
    }

    private sealed class ImportCounters
    {
        public int ShiftsCreated { get; set; }
        public int ShiftsReused { get; set; }
        public int WorkSchedulesCreated { get; set; }
        public int WorkSchedulesReused { get; set; }
        public int AssignmentsCreated { get; set; }
        public int AssignmentsReused { get; set; }
        public int RowsAssigned { get; set; }
        public int RowsWithoutScheduleData { get; set; }
        public int RowsUnresolved { get; set; }
        public int RowsSkipped { get; set; }
    }
}