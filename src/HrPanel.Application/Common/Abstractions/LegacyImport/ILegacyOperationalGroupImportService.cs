using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Common.Abstractions.LegacyImport
{
    public interface ILegacyOperationalGroupImportService
    {
        Task<LegacyOperationalGroupImportResult> ImportAsync(Guid batchId,CancellationToken cancellationToken = default);
    }
}
