using HrPanel.Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employees;

public sealed class EmployeePersonalDetailsConfiguration: IEntityTypeConfiguration<EmployeePersonalDetails>
{
    public void Configure(EntityTypeBuilder<EmployeePersonalDetails> builder)
    {
        builder.ToTable("EmployeePersonalDetails",DatabaseSchemas.Hr);

        builder.HasKey(details => details.EmployeeId);

        builder.Property(details => details.EmployeeId).ValueGeneratedNever();

        builder.Property(details => details.FirstName).HasMaxLength(EmployeeConstants.NameMaxLength).IsUnicode();

        builder.Property(details => details.LastName).HasMaxLength(EmployeeConstants.NameMaxLength).IsUnicode();

        builder.Property(details => details.FirstNameFa).HasMaxLength(EmployeeConstants.NameMaxLength).IsUnicode().IsRequired();

        builder.Property(details => details.LastNameFa).HasMaxLength(EmployeeConstants.NameMaxLength).IsUnicode().IsRequired();

        builder.Property(details => details.NationalCode).HasMaxLength(EmployeeConstants.NationalCodeLength).IsFixedLength().IsUnicode(false);

        builder.Property(details => details.FatherName).HasMaxLength(EmployeeConstants.NameMaxLength).IsUnicode();

        builder.Property(details => details.FatherNationalCode).HasMaxLength(EmployeeConstants.NationalCodeLength).IsFixedLength().IsUnicode(false);

        builder.Property(details => details.BirthDate).HasColumnType("date");

        builder.Property(details => details.BirthPlace).HasMaxLength(150).IsUnicode();

        builder.Property(details => details.Gender).HasConversion<short>().HasColumnType("smallint").IsRequired();

        builder.Property(details => details.MaritalStatus).HasConversion<short>().HasColumnType("smallint").IsRequired();

        // Shared primary-key one-to-one relationship
        builder.HasOne(details => details.Employee)
            .WithOne(employee => employee.PersonalDetails)
            .HasForeignKey<EmployeePersonalDetails>(
                details => details.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName(
                "FK_EmployeePersonalDetails_Employees");

        builder.HasIndex(details => details.NationalCode)
            .IsUnique()
            .HasFilter("[NationalCode] IS NOT NULL")
            .HasDatabaseName(
                "UX_EmployeePersonalDetails_NationalCode");
    }
}

// EmployeePersonalDetails.EmployeeId هم کلید اصلی و هم کلید خارجی به Employees.Id است
// به همین علت ValueGeneratedNever() کاملاً درست است

