namespace HrPanel.Application.Dtos.LegacyImport;

public sealed record LegacyEmploymentImportResult(Guid BatchId,int SourceRowCount,int EmploymentsCreated,int EmploymentsReused,int HrAssignmentsCreated,int HrAssignmentsReused,int CrAssignmentsCreated,int CrAssignmentsReused,int RowsSkipped,IReadOnlyCollection<string> Warnings);
