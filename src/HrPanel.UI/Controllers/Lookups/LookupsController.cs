using HrPanel.Application.Features.Lookups;
using HrPanel.UI.Models.ReferenceData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Lookups;

[AllowAnonymous]
public sealed class LookupsController : Controller
{
    private readonly ILookupService _service;
    public LookupsController(ILookupService service) => _service = service;

    [HttpGet("/reference-data")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) => View(new ReferenceDataViewModel(
        _service.GetEmployeeLookups(),
        await _service.GetEmploymentLookupsAsync(cancellationToken),
        await _service.GetOrganizationLookupsAsync(cancellationToken),
        await _service.GetSchedulingLookupsAsync(cancellationToken),
        await _service.GetAssetLookupsAsync(cancellationToken)));
}
