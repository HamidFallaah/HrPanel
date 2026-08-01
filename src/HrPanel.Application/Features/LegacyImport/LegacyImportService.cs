using FluentValidation;
using HrPanel.Application.Common.Abstractions.LegacyImport;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Common.Validation;
using HrPanel.Application.Dtos.LegacyImport;

namespace HrPanel.Application.Features.LegacyImport;

[Obsolete("Legacy import has already been completed. Keep this service only for support and recovery scenarios.")]
public sealed class LegacyImportService : ILegacyImportService
{
    private readonly ILegacyEmployeeImportService _employeeImportService;
    private readonly ILegacyOrganizationReferenceImportService _organizationImportService;
    private readonly ILegacyEmploymentImportService _employmentImportService;
    private readonly ILegacyRelationshipImportService _relationshipImportService;
    private readonly ILegacyEducationImportService _educationImportService;
    private readonly ILegacySchedulingImportService _schedulingImportService;
    private readonly ILegacyOperationalGroupImportService _operationalGroupImportService;
    private readonly IValidator<LegacyImportBatchDto> _batchValidator;

    public LegacyImportService(
        ILegacyEmployeeImportService employeeImportService,
        ILegacyOrganizationReferenceImportService organizationImportService,
        ILegacyEmploymentImportService employmentImportService,
        ILegacyRelationshipImportService relationshipImportService,
        ILegacyEducationImportService educationImportService,
        ILegacySchedulingImportService schedulingImportService,
        ILegacyOperationalGroupImportService operationalGroupImportService,
        IValidator<LegacyImportBatchDto> batchValidator)
    {
        _employeeImportService = employeeImportService;
        _organizationImportService = organizationImportService;
        _employmentImportService = employmentImportService;
        _relationshipImportService = relationshipImportService;
        _educationImportService = educationImportService;
        _schedulingImportService = schedulingImportService;
        _operationalGroupImportService = operationalGroupImportService;
        _batchValidator = batchValidator;
    }

    public async Task<Result<LegacyEmployeeImportResult>> ProcessEmployeeBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateBatchIdAsync(batchId, cancellationToken);

        if (validationError is not null)
        {
            return Result<LegacyEmployeeImportResult>.Failure(validationError);
        }

        var importResult = await _employeeImportService.ProcessBatchAsync(
            batchId,
            cancellationToken);

        if (importResult.TotalRows == 0)
        {
            return Result<LegacyEmployeeImportResult>.Failure(
                LegacyEmployeeImportErrors.BatchNotFound(batchId));
        }

        return Result<LegacyEmployeeImportResult>.Success(importResult);
    }

    public async Task<Result<OrganizationReferenceImportResult>> ProcessOrganizationBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateBatchIdAsync(batchId, cancellationToken);

        if (validationError is not null)
        {
            return Result<OrganizationReferenceImportResult>.Failure(validationError);
        }

        var importResult = await _organizationImportService.ImportAsync(
            batchId,
            cancellationToken);

        return Result<OrganizationReferenceImportResult>.Success(importResult);
    }

    public async Task<Result<LegacyEmploymentImportResult>> ProcessEmploymentBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateBatchIdAsync(batchId, cancellationToken);

        if (validationError is not null)
        {
            return Result<LegacyEmploymentImportResult>.Failure(validationError);
        }

        var importResult = await _employmentImportService.ImportAsync(
            batchId,
            cancellationToken);

        return Result<LegacyEmploymentImportResult>.Success(importResult);
    }

    public async Task<Result<LegacyRelationshipImportResult>> ProcessRelationshipBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateBatchIdAsync(batchId, cancellationToken);

        if (validationError is not null)
        {
            return Result<LegacyRelationshipImportResult>.Failure(validationError);
        }

        var importResult = await _relationshipImportService.ImportAsync(
            batchId,
            cancellationToken);

        return Result<LegacyRelationshipImportResult>.Success(importResult);
    }

    public async Task<Result<LegacyEducationImportResult>> ProcessEducationBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateBatchIdAsync(batchId, cancellationToken);

        if (validationError is not null)
        {
            return Result<LegacyEducationImportResult>.Failure(validationError);
        }

        var importResult = await _educationImportService.ImportAsync(
            batchId,
            cancellationToken);

        return Result<LegacyEducationImportResult>.Success(importResult);
    }

    public async Task<Result<LegacySchedulingImportResult>> ProcessSchedulingBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateBatchIdAsync(batchId, cancellationToken);

        if (validationError is not null)
        {
            return Result<LegacySchedulingImportResult>.Failure(validationError);
        }

        var importResult = await _schedulingImportService.ImportAsync(
            batchId,
            cancellationToken);

        return Result<LegacySchedulingImportResult>.Success(importResult);
    }

    public async Task<Result<LegacyOperationalGroupImportResult>> ProcessOperationalGroupBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateBatchIdAsync(batchId, cancellationToken);

        if (validationError is not null)
        {
            return Result<LegacyOperationalGroupImportResult>.Failure(validationError);
        }

        var importResult = await _operationalGroupImportService.ImportAsync(
            batchId,
            cancellationToken);

        return Result<LegacyOperationalGroupImportResult>.Success(importResult);
    }

    private async Task<ValidationError?> ValidateBatchIdAsync(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var validationResult = await _batchValidator.ValidateAsync(
            new LegacyImportBatchDto(batchId),
            cancellationToken);

        return validationResult.IsValid
            ? null
            : validationResult.ToValidationError();
    }
}
