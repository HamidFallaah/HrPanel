using HrPanel.Application.Common.Abstractions.Services;
using HrPanel.Application.Dtos.Identity;
using HrPanel.UI.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers;

public sealed class AuthController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous, HttpGet("/account/login")]
    public IActionResult LoginPage(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
        return View("Login", new LoginViewModel { ReturnUrl = returnUrl });
    }


    [ValidateAntiForgeryToken]
    [AllowAnonymous, HttpPost("/account/login")]
    public async Task<IActionResult> LoginPage(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View("Login", model);
        var result = await _authenticationService.LoginAsync(new LoginRequestDto
        {
            UserName = model.UserName,
            Password = model.Password,
            RememberMe = model.RememberMe
        }, cancellationToken);
        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) return LocalRedirect(model.ReturnUrl);
            return RedirectToAction("Index", "Dashboard");
        }
        ModelState.AddModelError(string.Empty, result.Status switch
        {
            LoginStatus.LockedOut => "حساب کاربری موقتاً قفل شده است؛ کمی بعد دوباره تلاش کنید",
            LoginStatus.NotAllowed => "ورود این حساب مجاز نیست",
            LoginStatus.RequiresTwoFactor => "این حساب به تأیید دومرحله‌ای نیاز دارد",
            _ => "نام کاربری یا رمز عبور نادرست است"
        });
        return View("Login", model);
    }


    [ValidateAntiForgeryToken]
    [Authorize, HttpPost("/account/logout")]
    public async Task<IActionResult> LogoutPage(CancellationToken cancellationToken)
    {
        await _authenticationService.LogoutAsync(cancellationToken);
        return RedirectToAction(nameof(LoginPage));
    }

    [Authorize, HttpGet("/account/change-password")]
    public IActionResult ChangePasswordPage() => View("ChangePassword", new ChangePasswordViewModel());


    [ValidateAntiForgeryToken]
    [Authorize, HttpPost("/account/change-password")]
    public async Task<IActionResult> ChangePasswordPage(ChangePasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View("ChangePassword", model);
        var result = await _authenticationService.ChangePasswordAsync(new ChangePasswordRequestDto
        {
            CurrentPassword = model.CurrentPassword,
            NewPassword = model.NewPassword,
            ConfirmNewPassword = model.ConfirmNewPassword
        }, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return View("ChangePassword", model);
        }
        TempData["SuccessMessage"] = "رمز عبور با موفقیت تغییر کرد";
        return RedirectToAction(nameof(ChangePasswordPage));
    }

    [AllowAnonymous, HttpGet("/account/access-denied")]
    public IActionResult AccessDeniedPage() => View("AccessDenied");
}
