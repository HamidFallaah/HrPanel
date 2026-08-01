using HrPanel.Application.Dtos.Assets;
using HrPanel.Application.Features.Assets;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Assets;

[ApiController]
[Route("api/assets")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class AssetsController : ControllerBase
{
    private readonly IAssetService _service;
    public AssetsController(IAssetService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetAssets([FromQuery] GetAssetsDto request,CancellationToken cancellationToken) => (await _service.GetAssetsAsync(request,cancellationToken)).ToActionResult(this);
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAsset(long id,CancellationToken cancellationToken) => (await _service.GetAssetAsync(id,cancellationToken)).ToActionResult(this);
    [HttpGet("/api/employees/{employeeId:long}/assets")]
    public async Task<IActionResult> GetEmployeeAssets(long employeeId,[FromQuery] GetAssetsDto request,CancellationToken cancellationToken)
    {
        var employeeAssets = request with { EmployeeId = employeeId };
        return (await _service.GetAssetsAsync(employeeAssets,cancellationToken)).ToActionResult(this);
    }
    [HttpPost]
    public async Task<IActionResult> CreateAsset([FromBody] CreateAssetDto request,CancellationToken cancellationToken) => (await _service.CreateAssetAsync(request,cancellationToken)).ToActionResult(this);
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsset(long id,[FromBody] UpdateAssetDto request,CancellationToken cancellationToken) => (await _service.UpdateAssetAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/assign")]
    public async Task<IActionResult> AssignAsset(long id,[FromBody] AssignAssetDto request,CancellationToken cancellationToken) => (await _service.AssignAssetAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/return")]
    public async Task<IActionResult> ReturnAsset(long id,[FromBody] ReturnAssetDto request,CancellationToken cancellationToken) => (await _service.ReturnAssetAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/maintenance")]
    public async Task<IActionResult> SendToMaintenance(long id,CancellationToken cancellationToken) => (await _service.SendToMaintenanceAsync(id,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/retire")]
    public async Task<IActionResult> RetireAsset(long id,CancellationToken cancellationToken) => (await _service.RetireAssetAsync(id,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/lost")]
    public async Task<IActionResult> MarkAssetAsLost(long id,CancellationToken cancellationToken) => (await _service.MarkAssetAsLostAsync(id,cancellationToken)).ToActionResult(this);
}
