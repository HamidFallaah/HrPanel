using HrPanel.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HrPanel.Persistence.Identity;

public static class IdentityModelConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>().ToTable("Users",DatabaseSchemas.Identity);

        modelBuilder.Entity<ApplicationRole>().ToTable("Roles",DatabaseSchemas.Identity);

        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles",DatabaseSchemas.Identity);

        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims",DatabaseSchemas.Identity);

        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins",DatabaseSchemas.Identity);

        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims",DatabaseSchemas.Identity);

        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens",DatabaseSchemas.Identity);
    }
}