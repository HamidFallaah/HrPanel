using HrPanel.Application.Features.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

[AllowAnonymous]
[Route("dashboard")]
public sealed class DashboardController(IDashboardService dashboardService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) => View(await dashboardService.GetAsync(cancellationToken));
}
