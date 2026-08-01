using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Application.Features.Employees;
using HrPanel.UI.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Employees;

[ApiController]
[Route("api/employees")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees([FromQuery] GetEmployeesDto request,CancellationToken cancellationToken)
        => (await _employeeService.GetEmployeesAsync(request,cancellationToken)).ToActionResult(this);

    [HttpGet("{employeeId:long}")]
    public async Task<IActionResult> GetEmployee(long employeeId,CancellationToken cancellationToken)
        => (await _employeeService.GetEmployeeDetailsAsync(employeeId,cancellationToken)).ToActionResult(this);

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto request,CancellationToken cancellationToken)
        => (await _employeeService.CreateEmployeeAsync(request,cancellationToken)).ToActionResult(this);

    [HttpPut("{employeeId:long}")]
    public async Task<IActionResult> UpdateEmployeeNumber(long employeeId,[FromBody] UpdateEmployeeNumberDto request,CancellationToken cancellationToken)
        => (await _employeeService.UpdateEmployeeNumberAsync(employeeId,request,cancellationToken)).ToActionResult(this);

    [HttpPut("{employeeId:long}/personal-details")]
    public async Task<IActionResult> UpdatePersonalDetails(long employeeId,[FromBody] UpdateEmployeePersonalDetailsDto request,CancellationToken cancellationToken)
        => (await _employeeService.UpdatePersonalDetailsAsync(employeeId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employeeId:long}/activate")]
    public async Task<IActionResult> ActivateEmployee(long employeeId,CancellationToken cancellationToken)
        => (await _employeeService.ActivateEmployeeAsync(employeeId,cancellationToken)).ToActionResult(this);

    [HttpPost("{employeeId:long}/deactivate")]
    public async Task<IActionResult> DeactivateEmployee(long employeeId,CancellationToken cancellationToken)
        => (await _employeeService.DeactivateEmployeeAsync(employeeId,cancellationToken)).ToActionResult(this);

    [HttpPost("{employeeId:long}/contacts")]
    public async Task<IActionResult> AddContact(long employeeId,[FromBody] AddEmployeeContactDto request,CancellationToken cancellationToken)
        => (await _employeeService.AddContactAsync(employeeId,request,cancellationToken)).ToActionResult(this);

    [HttpPut("{employeeId:long}/contacts/{contactId:long}")]
    public async Task<IActionResult> UpdateContact(long employeeId,long contactId,[FromBody] UpdateEmployeeContactDto request,CancellationToken cancellationToken)
        => (await _employeeService.UpdateContactAsync(employeeId,contactId,request,cancellationToken)).ToActionResult(this);

    [HttpDelete("{employeeId:long}/contacts/{contactId:long}")]
    public async Task<IActionResult> RemoveContact(long employeeId,long contactId,CancellationToken cancellationToken)
        => (await _employeeService.RemoveContactAsync(employeeId,contactId,cancellationToken)).ToActionResult(this);

    [HttpPost("{employeeId:long}/contacts/{contactId:long}/primary")]
    public async Task<IActionResult> SelectPrimaryContact(long employeeId,long contactId,CancellationToken cancellationToken)
        => (await _employeeService.SelectPrimaryContactAsync(employeeId,contactId,cancellationToken)).ToActionResult(this);

    [HttpPost("{employeeId:long}/identifiers")]
    public async Task<IActionResult> AddIdentifier(long employeeId,[FromBody] AddEmployeeIdentifierDto request,CancellationToken cancellationToken)
        => (await _employeeService.AddIdentifierAsync(employeeId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employeeId:long}/identifiers/{identifierId:long}/end")]
    public async Task<IActionResult> EndIdentifier(long employeeId,long identifierId,[FromBody] EndEmployeeIdentifierDto request,CancellationToken cancellationToken)
        => (await _employeeService.EndIdentifierAsync(employeeId,identifierId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employeeId:long}/education")]
    public async Task<IActionResult> AddEducation(long employeeId,[FromBody] AddEmployeeEducationDto request,CancellationToken cancellationToken)
        => (await _employeeService.AddEducationAsync(employeeId,request,cancellationToken)).ToActionResult(this);

    [HttpPut("{employeeId:long}/education/{educationId:long}")]
    public async Task<IActionResult> UpdateEducation(long employeeId,long educationId,[FromBody] AddEmployeeEducationDto request,CancellationToken cancellationToken)
        => (await _employeeService.UpdateEducationAsync(employeeId,educationId,request,cancellationToken)).ToActionResult(this);

    [HttpPost("{employeeId:long}/education/{educationId:long}/highest")]
    public async Task<IActionResult> SelectHighestEducation(long employeeId,long educationId,CancellationToken cancellationToken)
        => (await _employeeService.SelectHighestEducationAsync(employeeId,educationId,cancellationToken)).ToActionResult(this);

    [HttpDelete("{employeeId:long}/education/{educationId:long}")]
    public async Task<IActionResult> RemoveEducation(long employeeId,long educationId,CancellationToken cancellationToken)
        => (await _employeeService.RemoveEducationAsync(employeeId,educationId,cancellationToken)).ToActionResult(this);

    [HttpPost("{employeeId:long}/dependents")]
    public async Task<IActionResult> AddDependent(long employeeId,[FromBody] AddEmployeeDependentDto request,CancellationToken cancellationToken)
        => (await _employeeService.AddDependentAsync(employeeId,request,cancellationToken)).ToActionResult(this);

    [HttpPut("{employeeId:long}/dependents/{dependentId:long}")]
    public async Task<IActionResult> UpdateDependent(long employeeId,long dependentId,[FromBody] AddEmployeeDependentDto request,CancellationToken cancellationToken)
        => (await _employeeService.UpdateDependentAsync(employeeId,dependentId,request,cancellationToken)).ToActionResult(this);

    [HttpDelete("{employeeId:long}/dependents/{dependentId:long}")]
    public async Task<IActionResult> RemoveDependent(long employeeId,long dependentId,CancellationToken cancellationToken)
        => (await _employeeService.RemoveDependentAsync(employeeId,dependentId,cancellationToken)).ToActionResult(this);
}
