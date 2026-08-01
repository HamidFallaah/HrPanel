using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Dtos.Organization;
using HrPanel.Application.Features.Organization;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Organization;

[ApiController]
[Route("api/work-locations")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class WorkLocationsController : ControllerBase
{
    private readonly IOrganizationService _service;
    public WorkLocationsController(IOrganizationService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetWorkLocations([FromQuery] GetOrganizationItemsDto request,CancellationToken cancellationToken) => (await _service.GetWorkLocationsAsync(request,cancellationToken)).ToActionResult(this);
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetWorkLocation(long id,CancellationToken cancellationToken) => (await _service.GetWorkLocationAsync(id,cancellationToken)).ToActionResult(this);
    [HttpPost]
    public async Task<IActionResult> CreateWorkLocation([FromBody] SaveWorkLocationDto request,CancellationToken cancellationToken) => (await _service.CreateWorkLocationAsync(request,cancellationToken)).ToActionResult(this);
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateWorkLocation(long id,[FromBody] SaveWorkLocationDto request,CancellationToken cancellationToken) => (await _service.UpdateWorkLocationAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivateWorkLocation(long id,CancellationToken cancellationToken) => (await _service.ChangeWorkLocationStatusAsync(id,true,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateWorkLocation(long id,CancellationToken cancellationToken) => (await _service.ChangeWorkLocationStatusAsync(id,false,cancellationToken)).ToActionResult(this);
}
