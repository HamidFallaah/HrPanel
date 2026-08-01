using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Features.LegacyImport;

[Obsolete("Legacy import has already been completed. Keep this service only for support and recovery scenarios.")]
public interface ILegacyImportService
{
    Task<Result<LegacyEmployeeImportResult>> ProcessEmployeeBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<Result<OrganizationReferenceImportResult>> ProcessOrganizationBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<Result<LegacyEmploymentImportResult>> ProcessEmploymentBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<Result<LegacyRelationshipImportResult>> ProcessRelationshipBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<Result<LegacyEducationImportResult>> ProcessEducationBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<Result<LegacySchedulingImportResult>> ProcessSchedulingBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<Result<LegacyOperationalGroupImportResult>> ProcessOperationalGroupBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);
}
