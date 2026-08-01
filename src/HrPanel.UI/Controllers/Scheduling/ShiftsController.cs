using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Application.Features.Scheduling;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Scheduling;

[ApiController]
[Route("api/shifts")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class ShiftsController : ControllerBase
{
    private readonly ISchedulingService _service;
    public ShiftsController(ISchedulingService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetShifts([FromQuery] GetSchedulingItemsDto request,CancellationToken cancellationToken) => (await _service.GetShiftsAsync(request,cancellationToken)).ToActionResult(this);
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetShift(long id,CancellationToken cancellationToken) => (await _service.GetShiftAsync(id,cancellationToken)).ToActionResult(this);
    [HttpPost]
    public async Task<IActionResult> CreateShift([FromBody] SaveShiftDto request,CancellationToken cancellationToken) => (await _service.CreateShiftAsync(request,cancellationToken)).ToActionResult(this);
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateShift(long id,[FromBody] SaveShiftDto request,CancellationToken cancellationToken) => (await _service.UpdateShiftAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivateShift(long id,CancellationToken cancellationToken) => (await _service.ChangeShiftStatusAsync(id,true,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateShift(long id,CancellationToken cancellationToken) => (await _service.ChangeShiftStatusAsync(id,false,cancellationToken)).ToActionResult(this);
}
