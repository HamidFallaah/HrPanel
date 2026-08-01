using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Common.Abstractions.LegacyImport;

public interface ILegacyRelationshipImportService
{
    Task<LegacyRelationshipImportResult> ImportAsync(Guid batchId,CancellationToken cancellationToken = default);
}