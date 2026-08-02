using FluentValidation;
using HrPanel.Application.Common.Abstractions.Services;
using HrPanel.Application.Dtos.Identity;
using HrPanel.UI.Models.Authentication;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI.Controllers.Identity;

[ApiController]
[Authorize]
[IgnoreAntiforgeryToken]
[Route("api/auth")]
[ResponseCache(Location = ResponseCacheLocation.None,NoStore = true)]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator;
    private readonly IAntiforgery _antiforgery;
    public AuthController(IAuthenticationService authenticationService,IValidator<LoginRequestDto> loginValidator,IValidator<ChangePasswordRequestDto> changePasswordValidator, IAntiforgery antiforgery)
    {
        _authenticationService = authenticationService;
        _loginValidator = loginValidator;
        _changePasswordValidator = changePasswordValidator;
        _antiforgery = antiforgery;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(CurrentUserDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status423Locked)]
    public async Task<ActionResult<CurrentUserDto>> Login([FromBody] LoginRequestDto request,CancellationToken cancellationToken)
    {
        var validationResult = await _loginValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        var result = await _authenticationService.LoginAsync(request,cancellationToken);

        if (result.Succeeded && result.User is not null)
        {
            return Ok(result.User);
        }

        return result.Status switch
        {
            LoginStatus.InvalidCredentials => Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "ورود ناموفق",
                detail: "نام کاربری یا رمز عبور صحیح نیست"),

            LoginStatus.LockedOut => Problem(
                statusCode: StatusCodes.Status423Locked,
                title: "حساب کاربری قفل شده است",
                detail:"به دلیل چند تلاش ناموفق، حساب کاربری موقتاً قفل شده است"),

            LoginStatus.NotAllowed => Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "ورود مجاز نیست",
                detail: "این حساب در حال حاضر اجازه ورود به سامانه را ندارد"),

            LoginStatus.RequiresTwoFactor => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "تأیید دومرحله‌ای لازم است",
                detail: "برای ورود به این حساب، تأیید دومرحله‌ای باید انجام شود"),

            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "خطای ورود",
                detail: "در هنگام ورود به سامانه خطای غیرمنتظره‌ای رخ داد")
        };
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await _authenticationService.LogoutAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(CurrentUserDto),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CurrentUserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var currentUser = await _authenticationService.GetCurrentUserAsync(cancellationToken);

        if (currentUser is null)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized,title: "کاربر احراز هویت نشده است",detail: "ابتدا وارد سامانه شوید");
        }

        return Ok(currentUser);
    }

    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request,CancellationToken cancellationToken)
    {
        var validationResult = await _changePasswordValidator.ValidateAsync(request,cancellationToken);

        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        }

        var result = await _authenticationService.ChangePasswordAsync(request,cancellationToken);

        if (result.Succeeded)
        {
            return NoContent();
        }

        var errors = result.Errors.Count > 0? result.Errors.ToArray(): ["تغییر رمز عبور انجام نشد"];

        return BadRequest(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    ["Password"] = errors
                })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "تغییر رمز عبور انجام نشد."
            });
    }

    [AllowAnonymous]
    [HttpGet("csrf-token")]
    [ProducesResponseType(
    typeof(CsrfTokenResponse),StatusCodes.Status200OK)]
    public ActionResult<CsrfTokenResponse> GetCsrfToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        var requestToken = tokens.RequestToken ?? throw new InvalidOperationException("Antiforgery request token was not generated.");
        return Ok(new CsrfTokenResponse(requestToken));
    }
}