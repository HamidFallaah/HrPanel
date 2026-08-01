using HrPanel.Application.Common.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HrPanel.Persistence.Identity;

internal sealed class IdentitySeeder : IIdentitySeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentitySeedOptions _options;

    public IdentitySeeder(RoleManager<ApplicationRole> roleManager,UserManager<ApplicationUser> userManager,IOptions<IdentitySeedOptions> options)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _options = options.Value;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await EnsureRoleExistsAsync(RoleNames.Administrator,cancellationToken);

        await EnsureRoleExistsAsync(RoleNames.HrStaff,cancellationToken);

        if (!_options.CreateAdministrator)
        {
            return;
        }

        await EnsureAdministratorExistsAsync(cancellationToken);
    }

    private async Task EnsureRoleExistsAsync(string roleName,CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var role = new ApplicationRole(roleName);

        var result = await _roleManager.CreateAsync(role);

        EnsureSucceeded(result,$"Creating role '{roleName}'");
    }

    private async Task EnsureAdministratorExistsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userName = NormalizeOptionalValue(_options.AdministratorUserName);

        if (userName is null)
        {
            throw new InvalidOperationException("IdentitySeed:AdministratorUserName is required " +"when CreateAdministrator is true.");
        }

        var user = await _userManager.FindByNameAsync(userName);

        if (user is null)
        {
            user = await CreateAdministratorAsync(userName,cancellationToken);
        }

        var hasAdministratorRole = await _userManager.IsInRoleAsync(user,RoleNames.Administrator);

        if (hasAdministratorRole)
        {
            return;
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, RoleNames.Administrator);

        EnsureSucceeded(addToRoleResult,$"Assigning role '{RoleNames.Administrator}'");
    }

    private async Task<ApplicationUser> CreateAdministratorAsync(string userName,CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var password = NormalizeOptionalValue(_options.AdministratorPassword);

        if (password is null)
        {
            throw new InvalidOperationException("IdentitySeed:AdministratorPassword is required " + "when creating the administrator.");
        }

        var email = NormalizeOptionalValue(_options.AdministratorEmail);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            EmailConfirmed = email is not null,
            LockoutEnabled = true
        };

        user.SetDisplayName(_options.AdministratorDisplayName);

        var createResult = await _userManager.CreateAsync( user, password);

        EnsureSucceeded(createResult, $"Creating administrator '{userName}'");

        return user;
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null: value.Trim();
    }

    private static void EnsureSucceeded(IdentityResult result,string operation)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ",result.Errors.Select(error => $"{error.Code}: {error.Description}"));

        throw new InvalidOperationException($"{operation} failed. {errors}");
    }
}