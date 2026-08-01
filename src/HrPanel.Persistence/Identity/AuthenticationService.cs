using HrPanel.Application.Common.Abstractions.Services;
using HrPanel.Application.Dtos.Identity;
using Microsoft.AspNetCore.Identity;

namespace HrPanel.Persistence.Identity;

internal sealed class AuthenticationService: IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICurrentUserService _currentUserService;

    public AuthenticationService(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _currentUserService = currentUserService;
    }

    public async Task<LoginResultDto> LoginAsync(LoginRequestDto request,CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        var userName = request.UserName.Trim();

        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
        {
            return new LoginResultDto(LoginStatus.InvalidCredentials);
        }
        //lockoutOnFailure: مقدار true به این معنی است که رمزهای عبور نادرست جزو محدودیت پنج تلاش پیکربندی شده شما محسوب می‌شوند
        var signInResult = await _signInManager.PasswordSignInAsync(user,request.Password,request.RememberMe,lockoutOnFailure: true);

        if (signInResult.Succeeded)
        {
            var currentUser = await CreateCurrentUserDtoAsync(user,cancellationToken);

            return new LoginResultDto( LoginStatus.Succeeded,currentUser);
        }

        if (signInResult.IsLockedOut)
        {
            return new LoginResultDto(LoginStatus.LockedOut);
        }

        if (signInResult.IsNotAllowed)
        {
            return new LoginResultDto(LoginStatus.NotAllowed);
        }

        if (signInResult.RequiresTwoFactor)
        {
            return new LoginResultDto(LoginStatus.RequiresTwoFactor);
        }

        return new LoginResultDto(LoginStatus.InvalidCredentials);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _signInManager.SignOutAsync();
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var user = await FindCurrentUserAsync(cancellationToken);

        if (user is null)
        {
            return null;
        }

        return await CreateCurrentUserDtoAsync(user,cancellationToken);
    }

    public async Task<ChangePasswordResultDto> ChangePasswordAsync(ChangePasswordRequestDto request,CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await FindCurrentUserAsync(cancellationToken);

        if (user is null)
        {
            return ChangePasswordResultDto.Failure(
            [
                "کاربر وارد سیستم نشده است."
            ]);
        }

        var result = await _userManager.ChangePasswordAsync(user,request.CurrentPassword,request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description);

            return ChangePasswordResultDto.Failure(errors);
        }

        await _signInManager.RefreshSignInAsync(user);

        return ChangePasswordResultDto.Success();
    }

    private async Task<ApplicationUser?> FindCurrentUserAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_currentUserService.IsAuthenticated ||
            !_currentUserService.UserId.HasValue)
        {
            return null;
        }

        var userId = _currentUserService.UserId.Value.ToString();

        return await _userManager.FindByIdAsync(userId);
    }

    private async Task<CurrentUserDto> CreateCurrentUserDtoAsync(ApplicationUser user,CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserDto(
            UserId: user.Id,
            UserName: user.UserName ?? string.Empty,
            Email: user.Email,
            DisplayName: user.DisplayName,
            EmployeeId: user.EmployeeId,
            Roles: roles.ToArray());
    }
}