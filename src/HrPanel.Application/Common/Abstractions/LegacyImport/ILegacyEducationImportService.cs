using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Common.Abstractions.LegacyImport;

public interface ILegacyEducationImportService
{
    Task<LegacyEducationImportResult> ImportAsync(Guid batchId,CancellationToken cancellationToken = default);
}