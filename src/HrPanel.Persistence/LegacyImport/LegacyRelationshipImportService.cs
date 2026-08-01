using HrPanel.Application.Common.Abstractions.LegacyImport;
using HrPanel.Application.Dtos.LegacyImport;
using HrPanel.Domain.Employment;
using HrPanel.Domain.LegacyImport;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace HrPanel.Persistence.LegacyImport;

internal sealed class LegacyRelationshipImportService: ILegacyRelationshipImportService
{
    private const int LegacyUsernameMaxLength = 128;
    private readonly HrDbContext _dbContext;

    public LegacyRelationshipImportService(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LegacyRelationshipImportResult> ImportAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            _dbContext.ChangeTracker.Clear();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

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

            var employeesWithLegacyUsernames =
                await _dbContext.Employees
                    .AsNoTracking()
                    .Where(employee =>
                        employee.LegacyUserId != null)
                    .Select(employee => new
                    {
                        employee.Id,
                        employee.LegacyUserId
                    })
                    .ToListAsync(cancellationToken);

            var employeesByUsername =
                employeesWithLegacyUsernames
                    .GroupBy(
                        employee => employee.LegacyUserId!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Single().Id,
                        StringComparer.OrdinalIgnoreCase);

            var employments = await _dbContext.Employments
                .AsNoTracking()
                .Where(employment =>
                    employeeIds.Contains(employment.EmployeeId))
                .OrderByDescending(employment =>
                    employment.StartDate)
                .ToListAsync(cancellationToken);

            var employmentsByEmployeeId = employments
                .GroupBy(employment => employment.EmployeeId)
                .ToDictionary(
                    group => group.Key,
                    group => group.First());

            var counters = new ImportCounters();
            var warnings = new List<string>();
            var preparedRows = new List<PreparedRow>();

            foreach (var row in rows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!HasRelationshipData(row))
                {
                    continue;
                }

                if (!row.ImportedEmployeeId.HasValue)
                {
                    SkipRow(row, "ImportedEmployeeId is missing.", counters, warnings);
                    continue;
                }

                var employeeId = row.ImportedEmployeeId.Value;

                if (!employmentsByEmployeeId.TryGetValue(
                        employeeId,
                        out var employment))
                {
                    SkipRow(
                        row,
                        "No normalized employment was found. " + "Run or correct the employment import first.", counters, warnings);
                    continue;
                }

                var effectiveFrom = TryParseDate(row.StartWork, out var parsedStartDate) ? parsedStartDate : employment.StartDate;

                if (effectiveFrom < employment.StartDate)
                {
                    effectiveFrom = employment.StartDate;
                }

                DateOnly? effectiveTo = null;

                if (Clean(row.EndWork) is not null)
                {
                    if (!TryParseDate(
                            row.EndWork,
                            out var parsedEndDate))
                    {
                        SkipRow(row, "Relationship end date is invalid.", counters, warnings);
                        continue;
                    }

                    effectiveTo = parsedEndDate;
                }
                else if (employment.EndDate.HasValue)
                {
                    effectiveTo = employment.EndDate;
                }

                if (employment.EndDate.HasValue &&
                    (!effectiveTo.HasValue ||
                     effectiveTo > employment.EndDate.Value))
                {
                    effectiveTo = employment.EndDate;
                }

                if (employment.EndDate.HasValue &&
                    effectiveFrom > employment.EndDate.Value)
                {
                    SkipRow(row, "Relationship start date is after the " + "normalized employment end date.", counters, warnings);
                    continue;
                }

                if (effectiveTo.HasValue &&
                    effectiveTo.Value < effectiveFrom)
                {
                    SkipRow(row, $"Relationship end date " + $"'{effectiveTo:yyyy-MM-dd}' is before " + $"start date '{effectiveFrom:yyyy-MM-dd}'.", counters, warnings);

                    continue;
                }

                preparedRows.Add(new PreparedRow(row, employeeId, effectiveFrom, effectiveTo));
            }

            var referencedExternalUsernames = preparedRows
                .SelectMany(prepared =>
                    GetRelationshipSources(prepared.Row))
                .Select(source => source.Username)
                .Where(username =>
                    !employeesByUsername.ContainsKey(username))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var existingExternalPeople = await _dbContext.ExternalPersons.Where(person =>person.LegacyUsername != null).ToListAsync(cancellationToken);

            var externalPeopleByUsername = existingExternalPeople.ToDictionary(person => person.LegacyUsername!,StringComparer.OrdinalIgnoreCase);

            foreach (var username in referencedExternalUsernames)
            {
                if (username.Length > LegacyUsernameMaxLength)
                {
                    continue;
                }

                if (externalPeopleByUsername.ContainsKey(username))
                {
                    counters.ExternalPersonsReused++;
                    continue;
                }

                var externalPerson = ExternalPerson.Create( displayName: username, legacyUsername: username);

                _dbContext.ExternalPersons.Add(externalPerson);

                externalPeopleByUsername.Add(
                    username,
                    externalPerson);

                counters.ExternalPersonsCreated++;
            }

            // ExternalPerson IDs are required by the domain factory
            await _dbContext.SaveChangesAsync(cancellationToken);

            var preparedEmployeeIds = preparedRows.Select(row => row.EmployeeId).Distinct().ToArray();

            var knownRelationships =
                await _dbContext.EmployeeRelationships
                    .AsNoTracking()
                    .Where(relationship =>
                        preparedEmployeeIds.Contains(
                            relationship.EmployeeId))
                    .ToListAsync(cancellationToken);

            foreach (var prepared in preparedRows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var source in
                         GetRelationshipSources(prepared.Row))
                {
                    AddOrReuseRelationship(
                        prepared,
                        source,
                        employeesByUsername,
                        externalPeopleByUsername,
                        knownRelationships,
                        counters,
                        warnings);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new LegacyRelationshipImportResult(
                batchId,
                rows.Count,
                counters.GeneralRelationshipsCreated,
                counters.GeneralRelationshipsReused,
                counters.HrRelationshipsCreated,
                counters.HrRelationshipsReused,
                counters.CrRelationshipsCreated,
                counters.CrRelationshipsReused,
                counters.ExternalPersonsCreated,
                counters.ExternalPersonsReused,
                counters.RelationshipsSkipped,
                counters.RowsSkipped,
                warnings);
            });
    }
    private void AddOrReuseRelationship(
        PreparedRow prepared,
        RelationshipSource source,
        IReadOnlyDictionary<string, long> employeesByUsername,
        IReadOnlyDictionary<string, ExternalPerson> externalPeopleByUsername,
        ICollection<EmployeeRelationship> knownRelationships,
        ImportCounters counters,
        ICollection<string> warnings)
    {
        long? relatedEmployeeId = null;
        long? relatedExternalPersonId = null;

        if (employeesByUsername.TryGetValue(
                source.Username,
                out var resolvedEmployeeId))
        {
            relatedEmployeeId = resolvedEmployeeId;
        }
        else if (source.Username.Length >
                 LegacyUsernameMaxLength)
        {
            SkipRelationship(prepared.Row,source,$"username exceeds {LegacyUsernameMaxLength} characters.",counters,warnings);

            return;
        }
        else if (externalPeopleByUsername.TryGetValue(source.Username,out var externalPerson))
        {
            relatedExternalPersonId = externalPerson.Id;
        }
        else
        {
            SkipRelationship(prepared.Row,source,$"referenced username '{source.Username}' " +"could not be resolved.",counters,warnings);
            return;
        }

        if (relatedEmployeeId == prepared.EmployeeId)
        {
            SkipRelationship(prepared.Row, source,"self-relationship was ignored.",counters,warnings);

            return;
        }

        var exactRelationship = knownRelationships
            .FirstOrDefault(relationship =>
                relationship.EmployeeId ==
                prepared.EmployeeId &&
                relationship.Type == source.Type &&
                relationship.Context == source.Context &&
                relationship.RelatedEmployeeId ==
                relatedEmployeeId &&
                relationship.RelatedExternalPersonId ==
                relatedExternalPersonId &&
                relationship.EffectiveFrom ==
                prepared.EffectiveFrom &&
                relationship.EffectiveTo ==
                prepared.EffectiveTo);

        if (exactRelationship is not null)
        {
            IncrementReused(source.Context, counters);
            return;
        }

        var currentRelationship = knownRelationships
            .FirstOrDefault(relationship =>
                relationship.EmployeeId ==
                prepared.EmployeeId &&
                relationship.Type == source.Type &&
                relationship.Context == source.Context &&
                relationship.IsCurrent);

        if (currentRelationship is not null)
        {
            SkipRelationship(prepared.Row,source,"a different current normalized relationship already " +"exists and was preserved.",counters,warnings);

            return;
        }

        EmployeeRelationship relationship;

        if (relatedEmployeeId.HasValue)
        {
            relationship = EmployeeRelationship.ForEmployee(prepared.EmployeeId,source.Type,source.Context,relatedEmployeeId.Value,prepared.EffectiveFrom);
        }
        else
        {
            relationship =
                EmployeeRelationship.ForExternalPerson(prepared.EmployeeId,source.Type,source.Context,relatedExternalPersonId!.Value,prepared.EffectiveFrom);
        }

        if (prepared.EffectiveTo.HasValue)
        {
            relationship.End(prepared.EffectiveTo.Value);
        }

        _dbContext.EmployeeRelationships.Add(relationship);
        knownRelationships.Add(relationship);

        IncrementCreated(source.Context, counters);
    }

    private static IEnumerable<RelationshipSource> GetRelationshipSources(LegacyEmployeeImportRow row)
    {
        var sources = new[]
        {
            new RelationshipSource(
            RelationshipType.ManagerLevel2,
            RelationshipContext.Hr,
            row.ManagerUsername2),

            new RelationshipSource(
            RelationshipType.ManagerLevel3,
            RelationshipContext.Hr,
            row.ManagerUsername3),

            new RelationshipSource(
            RelationshipType.ManagerLevel4,
            RelationshipContext.Hr,
            row.ManagerUsername4),

            new RelationshipSource(
            RelationshipType.ManagerLevel2,
            RelationshipContext.Cr,
            row.ManagerUsernameCr2),

            new RelationshipSource(
            RelationshipType.ManagerLevel3,
            RelationshipContext.Cr,
            row.ManagerUsernameCr3),

            new RelationshipSource(
            RelationshipType.ManagerLevel4,
            RelationshipContext.Cr,
            row.ManagerUsernameCr4),
            new RelationshipSource(
              RelationshipType.SeniorManager,
              RelationshipContext.General,
              row.SeniorManager),
            new RelationshipSource(
                RelationshipType.Manager,
                RelationshipContext.Hr,
                row.ManagerUsername),

            new RelationshipSource(
                RelationshipType.Supervisor,
                RelationshipContext.Hr,
                row.SupervisorUsername),

            new RelationshipSource(
                RelationshipType.QualityAssurance,
                RelationshipContext.Hr,
                row.QaUsername),

            new RelationshipSource(
                RelationshipType.Manager,
                RelationshipContext.Cr,
                row.ManagerUsernameCr),

            new RelationshipSource(
                RelationshipType.Supervisor,
                RelationshipContext.Cr,
                row.SupervisorUsernameCr)
        };

        foreach (var source in sources)
        {
            var username = Clean(source.RawUsername);

            if (username is not null)
            {
                yield return source with
                {
                    Username = username
                };
            }
        }
    }

    private static bool HasRelationshipData(LegacyEmployeeImportRow row)
    {
        return new[]
        {
        row.SeniorManager,

        row.ManagerUsername,
        row.ManagerUsername2,
        row.ManagerUsername3,
        row.ManagerUsername4,
        row.SupervisorUsername,
        row.QaUsername,

        row.ManagerUsernameCr,
        row.ManagerUsernameCr2,
        row.ManagerUsernameCr3,
        row.ManagerUsernameCr4,
        row.SupervisorUsernameCr
    }.Any(value => Clean(value) is not null);
    }

    private static void IncrementCreated(RelationshipContext context,ImportCounters counters)
    {
        switch (context)
        {
            case RelationshipContext.General:
                counters.GeneralRelationshipsCreated++;
                break;

            case RelationshipContext.Hr:
                counters.HrRelationshipsCreated++;
                break;

            case RelationshipContext.Cr:
                counters.CrRelationshipsCreated++;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(context), context,"Unknown relationship context.");
        }
    }
    private static void IncrementReused(RelationshipContext context,ImportCounters counters)
    {
        switch (context)
        {
            case RelationshipContext.General:
                counters.GeneralRelationshipsReused++;
                break;

            case RelationshipContext.Hr:
                counters.HrRelationshipsReused++;
                break;

            case RelationshipContext.Cr:
                counters.CrRelationshipsReused++;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(context),context,"Unknown relationship context.");
        }
    }

    private static void SkipRow(LegacyEmployeeImportRow row,string reason,ImportCounters counters,ICollection<string> warnings)
    {
        counters.RowsSkipped++;
        warnings.Add($"Row {row.SourceRowNumber}: {reason}");
    }

    private static void SkipRelationship(LegacyEmployeeImportRow row,RelationshipSource source,string reason,ImportCounters counters,ICollection<string> warnings)
    {
        counters.RelationshipsSkipped++;
        warnings.Add($"Row {row.SourceRowNumber}: " +$"{source.Context} {source.Type} relationship: {reason}");
    }

    private static bool TryParseDate( string? value,out DateOnly date)
    {
        var cleaned = NormalizeDigits(Clean(value));

        if (cleaned is not null && DateTime.TryParse( cleaned,CultureInfo.GetCultureInfo("en-US"),DateTimeStyles.AllowWhiteSpaces,out var parsedDate))
        {
            date = DateOnly.FromDateTime(parsedDate);
            return true;
        }

        date = default;
        return false;
    }

    private static string? Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var cleaned = value.Trim();

        return cleaned.ToUpperInvariant() switch
        {
            "NULL" => null,
            "N/A" => null,
            "-" => null,
            "--" => null,
            _ => cleaned
        };
    }

    private static string? NormalizeDigits(string? value)
    {
        if (value is null)
        {
            return null;
        }

        return new string(value.Select(character => character switch
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
            }).ToArray());
    }

    private sealed record PreparedRow(LegacyEmployeeImportRow Row,long EmployeeId,DateOnly EffectiveFrom,DateOnly? EffectiveTo);
    private sealed record RelationshipSource(RelationshipType Type,RelationshipContext Context,string? RawUsername)
    {
        public string Username { get; init; } = string.Empty;
    }
    private sealed class ImportCounters
    {
        public int GeneralRelationshipsCreated { get; set; }
        public int GeneralRelationshipsReused { get; set; }
        public int HrRelationshipsCreated { get; set; }
        public int HrRelationshipsReused { get; set; }
        public int CrRelationshipsCreated { get; set; }
        public int CrRelationshipsReused { get; set; }
        public int ExternalPersonsCreated { get; set; }
        public int ExternalPersonsReused { get; set; }
        public int RelationshipsSkipped { get; set; }
        public int RowsSkipped { get; set; }
    }
}