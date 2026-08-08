using HrPanel.UI.Common.Constants;
using HrPanel.UI.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

[AllowAnonymous]
[Route("error")]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class ErrorsController : Controller
{
    [Route("")]
    public IActionResult Index()
    {
        return RenderError(StatusCodes.Status500InternalServerError);
    }

    [Route("{statusCode:int}")]
    public IActionResult Status(int statusCode)
    {
        return RenderError(statusCode);
    }

    private IActionResult RenderError(int statusCode)
    {
        var model = ErrorPageViewModel.FromStatusCode(statusCode);

        var originalPath = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Path ?? HttpContext.Features.Get<IStatusCodeReExecuteFeature>()?.OriginalPath;

        if (originalPath?.StartsWith("/api", StringComparison.OrdinalIgnoreCase) == true)
        {
            return Problem(statusCode: model.StatusCode,title: model.Title,detail: model.Message);
        }

        Response.StatusCode = model.StatusCode;

        ViewData[ViewDataKeys.Title] = model.Title;
        ViewData[ViewDataKeys.PageTitle] = model.Title;
        ViewData[ViewDataKeys.PageDescription] = null;
        ViewData[ViewDataKeys.Breadcrumbs] = new[]
        {
            new BreadcrumbItemViewModel("خطا")
        };

        return View("Error", model);
    }
}
