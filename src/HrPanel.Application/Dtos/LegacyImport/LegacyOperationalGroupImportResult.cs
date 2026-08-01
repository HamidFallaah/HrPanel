namespace HrPanel.Application.Dtos.LegacyImport;

public sealed record LegacyOperationalGroupImportResult
{
    public required Guid BatchId { get; init; }
    public int SourceRowsReviewed { get; init; }
    public int RowsWithPilot { get; init; }
    public int DistinctGroupsFound { get; init; }
    public int GroupsCreated { get; init; }
    public int GroupsReused { get; init; }
    public int AssignmentsCreated { get; init; }
    public int AssignmentsSkippedExisting { get; init; }
    public int RowsSkippedWithoutEmployment { get; init; }
    public IReadOnlyCollection<string> Warnings { get; init; } = [];
}
