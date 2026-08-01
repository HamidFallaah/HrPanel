using HrPanel.Application.Common.Abstractions.LegacyImport;
using HrPanel.Application.Dtos.LegacyImport;
using HrPanel.Domain.Employees;
using HrPanel.Domain.LegacyImport;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HrPanel.Persistence.LegacyImport;
internal sealed class LegacyEducationImportService: ILegacyEducationImportService
{
    private static readonly IReadOnlyDictionary<string, string> DegreeTitles = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["دیپلم"] = "دیپلم",
                ["کاردانی"] = "کاردانی",
                ["کارشناسی"] = "کارشناسی",
                ["کارشناسی ارشد"] = "کارشناسی ارشد",
                ["دکتری"] = "دکتری"
            };

    private static readonly IReadOnlyDictionary<string, string> FieldOfStudies = new Dictionary<string, string>(StringComparer.Ordinal)
         {
             ["کامپیوتر"] = "کامپیوتر",
             ["مدیریت"] = "مدیریت",
             ["حسابداری"] = "حسابداری",
             ["ریاضی"] = "ریاضی",
             ["سایر الکترونیک هواپیما"] = "سایر الکترونیک هواپیما",
             ["سایر تربیت بدنی وعلوم ورزشی"] = "سایر تربیت بدنی وعلوم ورزشی",
             ["سایر جغرافیای طبیعی"] = "سایر جغرافیای طبیعی",
             ["سایر شیمی"] = "سایر شیمی",
             ["سایر علوم اجتماعی"] = "سایر علوم اجتماعی",
             ["سایر گرافیک"] = "سایر گرافیک",
             ["سایر مترجمی همزمان زبان انگلیسی"] = "سایر مترجمی همزمان زبان انگلیسی",
             ["سایر معماری"] = "سایر معماری",
             ["سایر مهندسی موارد"] = "سایر مهندسی موارد",
             ["علوم اقتصادی"] = "علوم اقتصادی",
             ["علوم تربیتی"] = "علوم تربیتی",
             ["کشاورزی"] = "کشاورزی"
         };

    private readonly HrDbContext _dbContext;
    public LegacyEducationImportService(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LegacyEducationImportResult> ImportAsync(Guid batchId,CancellationToken cancellationToken = default)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            _dbContext.ChangeTracker.Clear();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable,cancellationToken);

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
                .Where(row =>
                    row.ImportedEmployeeId.HasValue)
                .Select(row =>
                    row.ImportedEmployeeId!.Value)
                .Distinct()
                .ToArray();

            var employees = await _dbContext.Employees
                .Include(employee =>
                    employee.EducationRecords)
                .Where(employee =>
                    employeeIds.Contains(employee.Id))
                .ToListAsync(cancellationToken);

            var employeesById = employees.ToDictionary(
                employee => employee.Id);

            var counters = new ImportCounters();
            var warnings = new List<string>();

            foreach (var row in rows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sourceValue = Clean(row.Education);

                if (sourceValue is null || IsLegacyNull(sourceValue))
                {
                    continue;
                }

                counters.RowsWithEducationData++;

                var normalizedValue = NormalizePersianText(sourceValue);

                string? degreeTitle = null;
                string? fieldOfStudy = null;

                if (DegreeTitles.TryGetValue(normalizedValue, out var recognizedDegreeTitle))
                {
                    degreeTitle = recognizedDegreeTitle;
                }
                else if (FieldOfStudies.TryGetValue( normalizedValue, out var recognizedFieldOfStudy))
                {
                    fieldOfStudy = recognizedFieldOfStudy;
                    counters.FieldOfStudyOnlyRows++;
                }
                else
                {
                    SkipRow(row, $"education value '{sourceValue}' is not recognized.", counters, warnings);
                    continue;
                }

                if (!row.ImportedEmployeeId.HasValue)
                {
                    SkipRow(row,"ImportedEmployeeId is missing.",counters,warnings);
                    continue;
                }

                if (!employeesById.TryGetValue(row.ImportedEmployeeId.Value,out var employee))
                {
                    SkipRow(row,"The normalized employee was not found.",counters, warnings);
                    continue;
                }

                var matchingEducation = employee.EducationRecords.FirstOrDefault( education => degreeTitle is not null ? education.DegreeTitle is not null && 
                NormalizePersianText( education.DegreeTitle) == degreeTitle : education.FieldOfStudy is not null &&
                NormalizePersianText( education.FieldOfStudy) == fieldOfStudy);

                if (matchingEducation is not null)
                {
                    counters.EducationRecordsReused++;
                    continue;
                }

                if (employee.EducationRecords.Count > 0)
                {
                    var importedValue = degreeTitle ?? fieldOfStudy;

                    SkipRow( row,"The employee already has normalized education data " + $"that differs from imported value '{importedValue}'. " +"Existing education data was preserved.", counters, warnings);

                    continue;
                }
                EmployeeEducation education;
                if (!string.IsNullOrWhiteSpace(degreeTitle))
                {
                    education = EmployeeEducation.Create(degreeTitle);
                }
                else if (!string.IsNullOrWhiteSpace(fieldOfStudy))
                {
                    education =  EmployeeEducation.CreateFieldOfStudyOnly(fieldOfStudy);
                }
                else
                {
                    throw new InvalidOperationException($"Row {row.SourceRowNumber}: both DegreeTitle and " + $"FieldOfStudy are empty. Source value: '{sourceValue}'.");
                }

                education.MarkAsHighestDegree();
                employee.AddEducation(education);
                counters.EducationRecordsCreated++;
            }

            await _dbContext.SaveChangesAsync( cancellationToken);

            await transaction.CommitAsync( cancellationToken);

            return new LegacyEducationImportResult(
                batchId,
                rows.Count,
                counters.RowsWithEducationData,
                counters.EducationRecordsCreated,
                counters.EducationRecordsReused,
                counters.FieldOfStudyOnlyRows,
                counters.RowsSkipped,
                warnings);
        });
    }

    private static bool IsLegacyNull( string value)
    {
        return value.Equals("NULL",StringComparison.OrdinalIgnoreCase) || value.Equals("N/A",StringComparison.OrdinalIgnoreCase) || value is "-" or "--";
    }

    private static string NormalizePersianText( string value)
    {
        var normalizedCharacters = value
            .Replace('ي', 'ی')
            .Replace('ى', 'ی')
            .Replace('ك', 'ک')
            .Replace('\u200C', ' ');

        return string.Join(' ', normalizedCharacters.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries));
    }

    private static string? Clean( string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null: value.Trim();
    }

    private static void SkipRow( LegacyEmployeeImportRow row, string reason, ImportCounters counters, ICollection<string> warnings)
    {
        counters.RowsSkipped++;

        warnings.Add($"Row {row.SourceRowNumber}: {reason}");
    }

    private sealed class ImportCounters
    {
        public int RowsWithEducationData { get; set; }
        public int EducationRecordsCreated { get; set; }
        public int EducationRecordsReused { get; set; }
        public int FieldOfStudyOnlyRows { get; set; }
        public int RowsSkipped { get; set; }
    }
}