using HrPanel.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Identity;

//This configuration creates a cross-schema relationship
//identity.Users.EmployeeId -> hr.Employees.Id

public sealed class ApplicationUserConfiguration: IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(user => user.DisplayName).HasMaxLength(150).IsUnicode();

        builder.HasOne(user => user.Employee).WithOne().HasForeignKey<ApplicationUser>(user => user.EmployeeId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(user => user.EmployeeId).IsUnique().HasFilter("[EmployeeId] IS NOT NULL").HasDatabaseName("UX_Users_EmployeeId");
    }
}