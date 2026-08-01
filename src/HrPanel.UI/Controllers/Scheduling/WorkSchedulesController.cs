using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Application.Features.Scheduling;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Scheduling;

[ApiController]
[Route("api/work-schedules")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class WorkSchedulesController : ControllerBase
{
    private readonly ISchedulingService _service;
    public WorkSchedulesController(ISchedulingService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetWorkSchedules([FromQuery] GetSchedulingItemsDto request,CancellationToken cancellationToken) => (await _service.GetWorkSchedulesAsync(request,cancellationToken)).ToActionResult(this);
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetWorkSchedule(long id,CancellationToken cancellationToken) => (await _service.GetWorkScheduleAsync(id,cancellationToken)).ToActionResult(this);
    [HttpPost]
    public async Task<IActionResult> CreateWorkSchedule([FromBody] CreateWorkScheduleDto request,CancellationToken cancellationToken) => (await _service.CreateWorkScheduleAsync(request,cancellationToken)).ToActionResult(this);
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateWorkSchedule(long id,[FromBody] UpdateWorkScheduleDto request,CancellationToken cancellationToken) => (await _service.UpdateWorkScheduleAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpPut("{id:long}/days")]
    public async Task<IActionResult> SetWorkScheduleDay(long id,[FromBody] SetWorkScheduleDayDto request,CancellationToken cancellationToken) => (await _service.SetWorkScheduleDayAsync(id,request,cancellationToken)).ToActionResult(this);
    [HttpDelete("{id:long}/days/{dayIndex:int}")]
    public async Task<IActionResult> RemoveWorkScheduleDay(long id,short dayIndex,CancellationToken cancellationToken) => (await _service.RemoveWorkScheduleDayAsync(id,dayIndex,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> ActivateWorkSchedule(long id,CancellationToken cancellationToken) => (await _service.ChangeWorkScheduleStatusAsync(id,true,cancellationToken)).ToActionResult(this);
    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> DeactivateWorkSchedule(long id,CancellationToken cancellationToken) => (await _service.ChangeWorkScheduleStatusAsync(id,false,cancellationToken)).ToActionResult(this);

    [HttpGet("/api/employments/{employmentId:long}/schedule-assignments")]
    public async Task<IActionResult> GetScheduleAssignments(long employmentId,CancellationToken cancellationToken) => (await _service.GetScheduleAssignmentsAsync(employmentId,cancellationToken)).ToActionResult(this);
    [HttpPost("/api/schedule-assignments")]
    public async Task<IActionResult> AssignWorkSchedule([FromBody] AssignWorkScheduleDto request,CancellationToken cancellationToken) => (await _service.AssignWorkScheduleAsync(request,cancellationToken)).ToActionResult(this);
    [HttpPost("/api/schedule-assignments/{assignmentId:long}/end")]
    public async Task<IActionResult> EndScheduleAssignment(long assignmentId,[FromBody] EndScheduleAssignmentDto request,CancellationToken cancellationToken) => (await _service.EndScheduleAssignmentAsync(assignmentId,request,cancellationToken)).ToActionResult(this);
}
