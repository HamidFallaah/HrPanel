using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Dtos.Organization;
using HrPanel.Application.Features.Organization;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Organization;

[ApiController]
[Route("api/organization-units")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class OrganizationUnitsController : ControllerBase
{
    private readonly IOrganizationService _service;
    public OrganizationUnitsController(IOrganizationService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetUnits([FromQuery] GetOrganizationUnitsDto request,CancellationToken cancellationToken) => (await _service.GetOrganizationUnitsAsync(request,cancellationToken)).ToActionResult(this);
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree(bool includeInactive,CancellationToken cancellationToken) => (await _service.GetOrganizationTreeAsync(includeInactive,cancellationToken)).ToActionResult(this);
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetUnit(long id,CancellationToken cancellationToken) => (await _service.GetOrganizationUnitAsync(id,cancellationToken)).ToActionResult(this);
    [HttpPost]
    public async Task<IActionResult> CreateUnit([FromBody] CreateOrganizationUnitDto request,CancellationToken cancellationToken) => (await _service.CreateOrganizationUnitAsync(request,cancellationToken)).ToActionResult(this);
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateUnit(long id,[FromBody] UpdateOrganizationUnitDto request,CancellationToken cancellationToken) => (await _service.UpdateOrganizationUnitAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/move")]
    public async Task<IActionResult> MoveUnit(long id,[FromBody] MoveOrganizationUnitDto request,CancellationToken cancellationToken) => (await _service.MoveOrganizationUnitAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivateUnit(long id,CancellationToken cancellationToken) => (await _service.ChangeOrganizationUnitStatusAsync(id,true,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateUnit(long id,CancellationToken cancellationToken) => (await _service.ChangeOrganizationUnitStatusAsync(id,false,cancellationToken)).ToActionResult(this);
}
