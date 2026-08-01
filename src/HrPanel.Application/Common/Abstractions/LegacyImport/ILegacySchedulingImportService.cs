using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Common.Abstractions.LegacyImport;
public interface ILegacySchedulingImportService
{
    Task<LegacySchedulingImportResult> ImportAsync(Guid batchId,CancellationToken cancellationToken = default);
}