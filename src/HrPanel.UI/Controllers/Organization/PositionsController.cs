using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Dtos.Organization;
using HrPanel.Application.Features.Organization;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Organization;

[ApiController]
[Route("api/positions")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class PositionsController : ControllerBase
{
    private readonly IOrganizationService _service;
    public PositionsController(IOrganizationService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetPositions([FromQuery] GetOrganizationItemsDto request,CancellationToken cancellationToken) => (await _service.GetPositionsAsync(request,cancellationToken)).ToActionResult(this);
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetPosition(long id,CancellationToken cancellationToken) => (await _service.GetPositionAsync(id,cancellationToken)).ToActionResult(this);
    [HttpPost]
    public async Task<IActionResult> CreatePosition([FromBody] SavePositionDto request,CancellationToken cancellationToken) => (await _service.CreatePositionAsync(request,cancellationToken)).ToActionResult(this);
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdatePosition(long id,[FromBody] SavePositionDto request,CancellationToken cancellationToken) => (await _service.UpdatePositionAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivatePosition(long id,CancellationToken cancellationToken) => (await _service.ChangePositionStatusAsync(id,true,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivatePosition(long id,CancellationToken cancellationToken) => (await _service.ChangePositionStatusAsync(id,false,cancellationToken)).ToActionResult(this);
}
