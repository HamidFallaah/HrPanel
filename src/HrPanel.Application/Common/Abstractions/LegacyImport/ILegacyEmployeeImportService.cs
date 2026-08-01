using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Common.Abstractions.LegacyImport;

public interface ILegacyEmployeeImportService
{
    Task<LegacyEmployeeImportResult> ProcessBatchAsync(Guid batchId,CancellationToken cancellationToken = default);
}