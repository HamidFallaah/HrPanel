using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Dtos.Organization;
using HrPanel.Application.Features.Organization;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Organization;

[ApiController]
[Route("api/operational-groups")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class OperationalGroupsController : ControllerBase
{
    private readonly IOrganizationService _service;
    public OperationalGroupsController(IOrganizationService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetOperationalGroups([FromQuery] GetOrganizationItemsDto request,CancellationToken cancellationToken) => (await _service.GetOperationalGroupsAsync(request,cancellationToken)).ToActionResult(this);
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetOperationalGroup(long id,CancellationToken cancellationToken) => (await _service.GetOperationalGroupAsync(id,cancellationToken)).ToActionResult(this);
    [HttpPost]
    public async Task<IActionResult> CreateOperationalGroup([FromBody] CreateOperationalGroupDto request,CancellationToken cancellationToken) => (await _service.CreateOperationalGroupAsync(request,cancellationToken)).ToActionResult(this);
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateOperationalGroup(long id,[FromBody] UpdateOperationalGroupDto request,CancellationToken cancellationToken) => (await _service.UpdateOperationalGroupAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivateOperationalGroup(long id,CancellationToken cancellationToken) => (await _service.ChangeOperationalGroupStatusAsync(id,true,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateOperationalGroup(long id,CancellationToken cancellationToken) => (await _service.ChangeOperationalGroupStatusAsync(id,false,cancellationToken)).ToActionResult(this);
}
