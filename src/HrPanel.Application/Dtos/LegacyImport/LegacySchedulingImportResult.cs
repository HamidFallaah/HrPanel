namespace HrPanel.Application.Dtos.LegacyImport;

public sealed record LegacySchedulingImportResult(
    Guid BatchId,
    int SourceRowCount,
    int ShiftsCreated,
    int ShiftsReused,
    int WorkSchedulesCreated,
    int WorkSchedulesReused,
    int AssignmentsCreated,
    int AssignmentsReused,
    int RowsAssigned,
    int RowsWithoutScheduleData,
    int RowsUnresolved,
    int RowsSkipped,
    IReadOnlyCollection<string> Warnings);
