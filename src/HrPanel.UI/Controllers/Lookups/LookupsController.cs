using HrPanel.Application.Common.Authorization;
using HrPanel.Application.Features.Lookups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Lookups;

[ApiController]
[Route("api/lookups")]
//[Authorize(Policy = PolicyNames.HrAccess)]
[AllowAnonymous]
public sealed class LookupsController : ControllerBase
{
    private readonly ILookupService _service;
    public LookupsController(ILookupService service) => _service = service;

    [HttpGet("employees")]
    public IActionResult GetEmployeeLookups() => Ok(_service.GetEmployeeLookups());
    [HttpGet("employment")]
    public async Task<IActionResult> GetEmploymentLookups(CancellationToken cancellationToken) => Ok(await _service.GetEmploymentLookupsAsync(cancellationToken));
    [HttpGet("organization")]
    public async Task<IActionResult> GetOrganizationLookups(CancellationToken cancellationToken) => Ok(await _service.GetOrganizationLookupsAsync(cancellationToken));
    [HttpGet("scheduling")]
    public async Task<IActionResult> GetSchedulingLookups(CancellationToken cancellationToken) => Ok(await _service.GetSchedulingLookupsAsync(cancellationToken));
    [HttpGet("assets")]
    public async Task<IActionResult> GetAssetLookups(CancellationToken cancellationToken) => Ok(await _service.GetAssetLookupsAsync(cancellationToken));
}
