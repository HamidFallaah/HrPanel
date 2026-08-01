using Microsoft.AspNetCore.Identity;

namespace HrPanel.Persistence.Identity;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {

    }
    public ApplicationRole(string roleName) : base(roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentException("نام نقش الزامی است", nameof(roleName));
        }

        var normalizedRoleName = roleName.Trim();

        Id = Guid.NewGuid();
        Name = normalizedRoleName;
        NormalizedName = normalizedRoleName.ToUpperInvariant();
    }
}