using HrPanel.Application.Dtos.Identity;

namespace HrPanel.Application.Common.Abstractions.Services;

public interface IAuthenticationService
{
    Task<LoginResultDto> LoginAsync(LoginRequestDto request,CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    Task<CurrentUserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    Task<ChangePasswordResultDto> ChangePasswordAsync(ChangePasswordRequestDto request,CancellationToken cancellationToken = default);
}