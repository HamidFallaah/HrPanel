using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

[AllowAnonymous]
public sealed class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Dashboard"): RedirectToAction("LoginPage", "Auth");
    }
}
