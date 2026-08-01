using System.Globalization;
using System.Security.Claims;
using HrPanel.Application.Common.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HrPanel.Persistence.Identity;
// Without this class, ICurrentUserService.EmployeeId would always be null
public sealed class ApplicationUserClaimsPrincipalFactory: UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager,IOptions<IdentityOptions> optionsAccessor): base(userManager, roleManager, optionsAccessor)
    {

    }

    protected override async Task<ClaimsIdentity>
        GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        if (user.EmployeeId.HasValue)
        {
            identity.AddClaim(new Claim(CustomClaimTypes.EmployeeId,user.EmployeeId.Value.ToString(CultureInfo.InvariantCulture)));
        }

        return identity;
    }
}