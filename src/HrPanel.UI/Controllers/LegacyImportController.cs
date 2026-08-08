using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Features.LegacyImport;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

[Obsolete("قبلا یه بار کال کردیم و دیتا وارد شده است پس نیازی بهش نداریم در حال حاضر")]
[ApiController]
[Route("api/legacy-import")]
[Authorize(Policy = PolicyNames.AdministratorOnly)]
public sealed class LegacyImportController : ControllerBase
{
    private readonly ILegacyImportService _legacyImportService;

    public LegacyImportController(ILegacyImportService legacyImportService)
    {
        _legacyImportService = legacyImportService;
    }

    [HttpPost("employees/{batchId:guid}")]
    public async Task<IActionResult> ProcessEmployeeBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _legacyImportService.ProcessEmployeeBatchAsync(
            batchId,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("organization/{batchId:guid}")]
    public async Task<IActionResult> ProcessOrganizationBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _legacyImportService.ProcessOrganizationBatchAsync(
            batchId,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("employment/{batchId:guid}")]
    public async Task<IActionResult> ProcessEmploymentBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _legacyImportService.ProcessEmploymentBatchAsync(
            batchId,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("relationships/{batchId:guid}")]
    public async Task<IActionResult> ProcessRelationshipBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _legacyImportService.ProcessRelationshipBatchAsync(
            batchId,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("education/{batchId:guid}")]
    public async Task<IActionResult> ProcessEducationBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _legacyImportService.ProcessEducationBatchAsync(
            batchId,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("scheduling/{batchId:guid}")]
    public async Task<IActionResult> ProcessSchedulingBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _legacyImportService.ProcessSchedulingBatchAsync(
            batchId,
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("operational-groups/{batchId:guid}")]
    public async Task<IActionResult> ProcessOperationalGroupBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _legacyImportService.ProcessOperationalGroupBatchAsync(
            batchId,
            cancellationToken);

        return result.ToActionResult(this);
    }
}
