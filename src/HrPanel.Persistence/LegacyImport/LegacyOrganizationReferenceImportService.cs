using HrPanel.Application.Common.Abstractions.LegacyImport;
using HrPanel.Application.Dtos.LegacyImport;
using HrPanel.Domain.LegacyImport;
using HrPanel.Domain.Organization;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace HrPanel.Persistence.LegacyImport;

public sealed class LegacyOrganizationReferenceImportService: ILegacyOrganizationReferenceImportService
{
    private const string CompanyCode = "IRANCELL";
    private const string HrBranchCode = "IRANCELL-HR";
    private const string CrBranchCode = "IRANCELL-CR";

    private readonly HrDbContext _dbContext;

    public LegacyOrganizationReferenceImportService(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrganizationReferenceImportResult> ImportAsync(Guid batchId,CancellationToken cancellationToken = default)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable,cancellationToken);

            try
            {
                var result = await ImportOrganizationReferencesInternalAsync(batchId,cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    private async Task<OrganizationReferenceImportResult>ImportOrganizationReferencesInternalAsync(Guid batchId,CancellationToken cancellationToken)
    {
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
            throw new InvalidOperationException( $"No imported staging rows were found for batch '{batchId}'.");
        }

        var unitTypes = await _dbContext.OrganizationUnitTypes
            .AsNoTracking()
            .ToDictionaryAsync(
                type => type.Code,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        string[] requiredTypeCodes =
        [
            "COMPANY",
            "DIVISION",
            "SUBDIVISION",
            "DEPARTMENT",
            "SECTION",
        ];

        var missingTypeCodes = requiredTypeCodes.Where(code => !unitTypes.ContainsKey(code)).ToArray();

        if (missingTypeCodes.Length > 0)
        {
            throw new InvalidOperationException("Missing organization-unit types: " + $"{string.Join(", ", missingTypeCodes)}.");
        }

        var counters = new ImportCounters();

        var warnings = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        var existingUnitCodeList =
            await _dbContext.OrganizationUnits
                .Select(unit => unit.Code)
                .ToListAsync(cancellationToken);

        var existingUnitCodes = existingUnitCodeList
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var reusedUnitCodes = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        var company = await GetOrCreateUnitAsync(
            unitTypes["COMPANY"].Id,
            CompanyCode,
            "ایرانسل",
            "Irancell",
            parentId: null,
            existingUnitCodes,
            reusedUnitCodes,
            counters,
            cancellationToken);

        // The HR and CR branches are separate so that identical
        // legacy names are never merged into the same hierarchy
        var hrBranch = await GetOrCreateUnitAsync(
            unitTypes["DIVISION"].Id,
            HrBranchCode,
            "ساختار منابع انسانی",
            "HR",
            company.Id,
            existingUnitCodes,
            reusedUnitCodes,
            counters,
            cancellationToken);

        var crBranch = await GetOrCreateUnitAsync(
            unitTypes["DIVISION"].Id,
            CrBranchCode,
            "ساختار CR",
            "CR",
            company.Id,
            existingUnitCodes,
            reusedUnitCodes,
            counters,
            cancellationToken);

        foreach (var row in rows)
        {
            var hasHrOrganization = HasValue(row.Division) || HasValue(row.SubDivision) || HasValue(row.Department) || HasValue(row.Section);

            var hasCrOrganization = HasValue(row.DivisionCr);

            if (!hasHrOrganization && !hasCrOrganization)
            {
                counters.RowsWithoutOrganizationData++;
            }

            if (hasHrOrganization)
            {
                await ImportHierarchyAsync(
                    "HR",
                    hrBranch,
                    [
                        new HierarchyPart( "DIVISION", row.Division),
                        new HierarchyPart( "SUBDIVISION", row.SubDivision),
                        new HierarchyPart( "DEPARTMENT", row.Department),
                        new HierarchyPart( "SECTION", row.Section)
                    ],
                    unitTypes,
                    existingUnitCodes,
                    reusedUnitCodes,
                    counters,
                    cancellationToken);
            }

            if (hasCrOrganization)
            {
                await ImportHierarchyAsync(
                    "CR",
                    crBranch,
                    [
                        new HierarchyPart(
                            "DIVISION",
                            row.DivisionCr)
                    ],
                    unitTypes,
                    existingUnitCodes,
                    reusedUnitCodes,
                    counters,
                    cancellationToken);
            }
        }

        await ImportPositionsAsync(rows,counters,cancellationToken);

        await ImportWorkLocationsAsync(rows,counters,warnings,cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new OrganizationReferenceImportResult(
            batchId,
            rows.Count,
            counters.OrganizationUnitsCreated,
            counters.OrganizationUnitsReused,
            counters.PositionsCreated,
            counters.PositionsReused,
            counters.WorkLocationsCreated,
            counters.WorkLocationsReused,
            counters.RowsWithoutOrganizationData,
            warnings
                .OrderBy(warning => warning)
                .ToArray());
    }

    private async Task ImportHierarchyAsync(
        string tree,
        OrganizationUnit branch,
        IEnumerable<HierarchyPart> parts,
        IReadOnlyDictionary<string, OrganizationUnitType> unitTypes,
        HashSet<string> existingUnitCodes,
        HashSet<string> reusedUnitCodes,
        ImportCounters counters,
        CancellationToken cancellationToken)
    {
        var parent = branch;
        var path = new List<string> { tree };

        foreach (var part in parts)
        {
            var name = Clean(part.Name);

            if (name is null)
            {
                continue;
            }

            path.Add( $"{part.TypeCode}:{Normalize(name)}");

            var code = CreateStableCode( $"OU-{tree}", string.Join("/", path));

            parent = await GetOrCreateUnitAsync(
                unitTypes[part.TypeCode].Id,
                code,
                name,
                name,
                parent.Id,
                existingUnitCodes,
                reusedUnitCodes,
                counters,
                cancellationToken);
        }
    }

    private async Task<OrganizationUnit> GetOrCreateUnitAsync(
        short typeId,
        string code,
        string nameFa,
        string? nameEn,
        long? parentId,
        HashSet<string> existingUnitCodes,
        HashSet<string> reusedUnitCodes,
        ImportCounters counters,
        CancellationToken cancellationToken)
    {
        var existing =
            await _dbContext.OrganizationUnits
                .SingleOrDefaultAsync(
                    unit => unit.Code == code,
                    cancellationToken);

        if (existing is not null)
        {
            if (existing.OrganizationUnitTypeId != typeId ||
                existing.ParentOrganizationUnitId != parentId)
            {
                throw new InvalidOperationException( $"Organization-unit code '{code}' exists " +"with a different type or parent.");
            }

            if (reusedUnitCodes.Add(code))
            {
                counters.OrganizationUnitsReused++;
            }

            return existing;
        }

        var unit = OrganizationUnit.Create(
            typeId,
            code,
            nameFa,
            nameEn,
            parentId);

        _dbContext.OrganizationUnits.Add(unit);

        // Save here because child records require the generated
        // identity value of their parent
        await _dbContext.SaveChangesAsync(cancellationToken);

        existingUnitCodes.Add(code);
        counters.OrganizationUnitsCreated++;

        return unit;
    }

    private async Task ImportPositionsAsync(IReadOnlyCollection<LegacyEmployeeImportRow> rows, ImportCounters counters,CancellationToken cancellationToken)
    {
        var sourceNames = rows
            .SelectMany(row => new[]
            {
                Clean(row.PositionHr),
                Clean(row.PositionCr)
            })
            .Where(name => name is not null)
            .Select(name => name!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToArray();

        var existingCodeList = await _dbContext.Positions
            .Select(position => position.Code)
            .ToListAsync(cancellationToken);

        var existingCodes = existingCodeList
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var name in sourceNames)
        {
            var code = CreateStableCode(
                "POS",
                Normalize(name));

            if (existingCodes.Contains(code))
            {
                counters.PositionsReused++;
                continue;
            }

            _dbContext.Positions.Add( Position.Create( code, name, name));

            existingCodes.Add(code);
            counters.PositionsCreated++;
        }
    }

    private async Task ImportWorkLocationsAsync(
        IReadOnlyCollection<LegacyEmployeeImportRow> rows,
        ImportCounters counters,
        HashSet<string> warnings,
        CancellationToken cancellationToken)
    {
        var locations = rows
            .Select(row => new LocationSource(
                Clean(row.WorkLocation),
                Clean(row.ProvinceWork),
                Clean(row.CityWork)))
            .Where(location =>
                location.Name is not null ||
                location.Province is not null ||
                location.City is not null)
            .GroupBy(
                location =>
                    $"{Normalize(location.Name)}|" +
                    $"{Normalize(location.Province)}|" +
                    $"{Normalize(location.City)}",
                StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();

        var existingCodeList = await _dbContext.WorkLocations .Select(location => location.Code) .ToListAsync(cancellationToken);

        var existingCodes = existingCodeList
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var location in locations)
        {
            var displayName =
                location.Name ??
                location.City ??
                location.Province!;

            var identity = $"{Normalize(location.Name)}|" + $"{Normalize(location.Province)}|" + $"{Normalize(location.City)}";

            var code = CreateStableCode(
                "LOC",
                identity);

            if (existingCodes.Contains(code))
            {
                counters.WorkLocationsReused++;
                continue;
            }

            if (location.Name is null)
            {
                warnings.Add( $"Work location '{displayName}' was derived " +"from city/province because WorkLocation was empty.");
            }

            _dbContext.WorkLocations.Add(WorkLocation.Create(code,displayName,displayName,location.Province,location.City));

            existingCodes.Add(code);
            counters.WorkLocationsCreated++;
        }
    }

    private static string CreateStableCode( string prefix,string identity)
    {
        var hash = SHA256.HashData( Encoding.UTF8.GetBytes(identity));

        return $"{prefix}-{Convert.ToHexString(hash)[..20]}";
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)? null: value.Trim();
    }

    private static string Normalize(string? value)
    {
        return Clean(value)?.ToUpperInvariant() ?? string.Empty;
    }

    private static bool HasValue(string? value)
    {
        return Clean(value) is not null;
    }

    private sealed record HierarchyPart(string TypeCode,string? Name);

    private sealed record LocationSource(string? Name,string? Province,string? City);

    private sealed class ImportCounters
    {
        public int OrganizationUnitsCreated { get; set; }
        public int OrganizationUnitsReused { get; set; }
        public int PositionsCreated { get; set; }
        public int PositionsReused { get; set; }
        public int WorkLocationsCreated { get; set; }
        public int WorkLocationsReused { get; set; }
        public int RowsWithoutOrganizationData { get; set; }
    }
}