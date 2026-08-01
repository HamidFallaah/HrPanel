namespace HrPanel.Application.Dtos.LegacyImport;

public sealed record LegacyRelationshipImportResult(
    Guid BatchId,
    int SourceRowCount,
    int GeneralRelationshipsCreated,
    int GeneralRelationshipsReused,
    int HrRelationshipsCreated,
    int HrRelationshipsReused,
    int CrRelationshipsCreated,
    int CrRelationshipsReused,
    int ExternalPersonsCreated,
    int ExternalPersonsReused,
    int RelationshipsSkipped,
    int RowsSkipped,
    IReadOnlyCollection<string> Warnings);
