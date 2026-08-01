namespace HrPanel.Application.Dtos.LegacyImport;

public sealed record LegacyEducationImportResult(Guid BatchId,int SourceRowCount,int RowsWithEducationData,int EducationRecordsCreated,int EducationRecordsReused,int FieldOfStudyOnlyRows,int RowsSkipped,IReadOnlyCollection<string> Warnings);
