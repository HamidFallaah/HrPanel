using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Common.Abstractions.LegacyImport;

public interface ILegacyEmploymentImportService
{
    Task<LegacyEmploymentImportResult> ImportAsync(Guid batchId,CancellationToken cancellationToken = default);
}