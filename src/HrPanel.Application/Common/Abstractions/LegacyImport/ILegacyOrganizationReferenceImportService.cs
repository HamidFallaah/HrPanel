using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Common.Abstractions.LegacyImport;

public interface ILegacyOrganizationReferenceImportService
{
    Task<OrganizationReferenceImportResult> ImportAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);
}
