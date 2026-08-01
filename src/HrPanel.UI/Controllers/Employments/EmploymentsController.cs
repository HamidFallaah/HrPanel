using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Dtos.Employments;
using HrPanel.Application.Features.Employments;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Employments;

[ApiController]
[Route("api/employments")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class EmploymentsController : ControllerBase
{
    private readonly IEmploymentService _service;
    public EmploymentsController(IEmploymentService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetEmployments([FromQuery] GetEmploymentsDto request,CancellationToken cancellationToken)
        => (await _service.GetEmploymentsAsync(request,cancellationToken)).ToActionResult(this);

    [HttpGet("{employmentId:long}")]
    public async Task<IActionResult> GetEmployment(long employmentId,CancellationToken cancellationToken)
        => (await _service.GetEmploymentAsync(employmentId,cancellationToken)).ToActionResult(this);

    [HttpPost]
    public async Task<IActionResult> StartEmployment([FromBody] StartEmploymentDto request,CancellationToken cancellationToken)
        => (await _service.StartEmploymentAsync(request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employmentId:long}/change-status")]
    public async Task<IActionResult> ChangeStatus(long employmentId,[FromBody] ChangeEmploymentStatusDto request,CancellationToken cancellationToken)
        => (await _service.ChangeStatusAsync(employmentId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employmentId:long}/change-work-time-type")]
    public async Task<IActionResult> ChangeWorkTimeType(long employmentId,[FromBody] ChangeWorkTimeTypeDto request,CancellationToken cancellationToken)
        => (await _service.ChangeWorkTimeTypeAsync(employmentId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employmentId:long}/end")]
    public async Task<IActionResult> EndEmployment(long employmentId,[FromBody] EndEmploymentDto request,CancellationToken cancellationToken)
        => (await _service.EndEmploymentAsync(employmentId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employmentId:long}/assignments")]
    public async Task<IActionResult> AddAssignment(long employmentId,[FromBody] AddEmployeeAssignmentDto request,CancellationToken cancellationToken)
        => (await _service.AddAssignmentAsync(employmentId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employmentId:long}/assignments/{assignmentId:long}/end")]
    public async Task<IActionResult> EndAssignment(long employmentId,long assignmentId,[FromBody] EndAssignmentDto request,CancellationToken cancellationToken)
        => (await _service.EndAssignmentAsync(employmentId,assignmentId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employmentId:long}/operational-groups")]
    public async Task<IActionResult> AssignOperationalGroup(long employmentId,[FromBody] AssignOperationalGroupDto request,CancellationToken cancellationToken)
        => (await _service.AssignOperationalGroupAsync(employmentId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employmentId:long}/operational-groups/{assignmentId:long}/primary")]
    public async Task<IActionResult> SelectPrimaryOperationalGroup(long employmentId,long assignmentId,CancellationToken cancellationToken)
        => (await _service.SelectPrimaryOperationalGroupAsync(employmentId,assignmentId,cancellationToken)).ToActionResult(this);

    [HttpPost("{employmentId:long}/operational-groups/{assignmentId:long}/end")]
    public async Task<IActionResult> EndOperationalGroupAssignment(long employmentId,long assignmentId,[FromBody] EndAssignmentDto request,CancellationToken cancellationToken)
        => (await _service.EndOperationalGroupAssignmentAsync(employmentId,assignmentId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("/api/employees/{employeeId:long}/relationships")]
    public async Task<IActionResult> AddRelationship(long employeeId,[FromBody] AddEmployeeRelationshipDto request,CancellationToken cancellationToken)
        => (await _service.AddRelationshipAsync(employeeId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("/api/employees/{employeeId:long}/relationships/{relationshipId:long}/end")]
    public async Task<IActionResult> EndRelationship(long employeeId,long relationshipId,[FromBody] EndAssignmentDto request,CancellationToken cancellationToken)
        => (await _service.EndRelationshipAsync(employeeId,relationshipId,request,cancellationToken)).ToActionResult(this);

    [HttpGet("external-persons")]
    public async Task<IActionResult> GetExternalPersons(string? search,bool? isActive,CancellationToken cancellationToken)
        => (await _service.GetExternalPersonsAsync(search,isActive,cancellationToken)).ToActionResult(this);

    [HttpPost("external-persons")]
    public async Task<IActionResult> CreateExternalPerson([FromBody] CreateExternalPersonDto request,CancellationToken cancellationToken)
        => (await _service.CreateExternalPersonAsync(request,cancellationToken)).ToActionResult(this);

    [HttpPut("external-persons/{externalPersonId:long}")]
    public async Task<IActionResult> UpdateExternalPerson(long externalPersonId,[FromBody] UpdateExternalPersonDto request,CancellationToken cancellationToken)
        => (await _service.UpdateExternalPersonAsync(externalPersonId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("external-persons/{externalPersonId:long}/activate")]
    public async Task<IActionResult> ActivateExternalPerson(long externalPersonId,CancellationToken cancellationToken)
        => (await _service.ChangeExternalPersonStatusAsync(externalPersonId,true,cancellationToken)).ToActionResult(this);

    [HttpPost("external-persons/{externalPersonId:long}/deactivate")]
    public async Task<IActionResult> DeactivateExternalPerson(long externalPersonId,CancellationToken cancellationToken)
        => (await _service.ChangeExternalPersonStatusAsync(externalPersonId,false,cancellationToken)).ToActionResult(this);

    [HttpPost("disciplinary-actions")]
    public async Task<IActionResult> AddDisciplinaryAction([FromBody] AddDisciplinaryActionDto request,CancellationToken cancellationToken)
        => (await _service.AddDisciplinaryActionAsync(request,cancellationToken)).ToActionResult(this);

    [HttpPost("/api/employees/{employeeId:long}/disciplinary-actions/{actionId:long}/close")]
    public async Task<IActionResult> CloseDisciplinaryAction(long employeeId,long actionId,[FromBody] CloseDisciplinaryActionDto request,CancellationToken cancellationToken)
        => (await _service.CloseDisciplinaryActionAsync(employeeId,actionId,request,cancellationToken)).ToActionResult(this);
}
