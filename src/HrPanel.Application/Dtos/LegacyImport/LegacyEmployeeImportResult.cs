namespace HrPanel.Application.Dtos.LegacyImport;

public sealed record LegacyEmployeeImportResult(Guid BatchId,int TotalRows,int PendingRows,int ProcessingRows,int ImportedRows,int FailedRows,int SkippedRows);
