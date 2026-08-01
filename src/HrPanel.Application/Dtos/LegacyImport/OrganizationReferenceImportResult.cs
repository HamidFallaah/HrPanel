namespace HrPanel.Application.Dtos.LegacyImport;

public sealed record OrganizationReferenceImportResult(
    Guid BatchId,
    int SourceRowCount,
    int OrganizationUnitsCreated,
    int OrganizationUnitsReused,
    int PositionsCreated,
    int PositionsReused,
    int WorkLocationsCreated,
    int WorkLocationsReused,
    int RowsWithoutOrganizationData,
    IReadOnlyCollection<string> Warnings)
{
    public int TotalOrganizationUnits => OrganizationUnitsCreated + OrganizationUnitsReused;
    public int TotalPositions => PositionsCreated + PositionsReused;
    public int TotalWorkLocations => WorkLocationsCreated + WorkLocationsReused;
}
