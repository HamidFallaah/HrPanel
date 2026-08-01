using HrPanel.Domain.Employees;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employees;

public sealed class EmployeeEducationConfiguration: IEntityTypeConfiguration<EmployeeEducation>
{
    public void Configure(EntityTypeBuilder<EmployeeEducation> builder)
    {
        builder.ToTable("EmployeeEducations",DatabaseSchemas.Hr);

        builder.HasKey(education => education.Id);

        builder.Property(education => education.Id).UseIdentityColumn();

        builder.Property(education => education.DegreeTitle).HasMaxLength( EmployeeConstants.EducationTitleMaxLength).IsUnicode().IsRequired(false);

        builder.Property(education => education.FieldOfStudy).HasMaxLength(EmployeeConstants.EducationTitleMaxLength).IsUnicode();

        builder.Property(education => education.InstitutionName).HasMaxLength(EmployeeConstants.InstitutionNameMaxLength).IsUnicode();

        builder.Property(education => education.GraduationDate).HasColumnType("date");

        builder.Property(education => education.IsHighestDegree).HasDefaultValue(false).IsRequired();

        builder.HasIndex(education => education.EmployeeId).HasDatabaseName("IX_EmployeeEducations_EmployeeId");

        builder.HasIndex(education => education.EmployeeId).IsUnique().HasFilter("[IsHighestDegree] = 1").HasDatabaseName("UX_EmployeeEducations_HighestDegree");
        
        //The filtered unique index allows several educational records for an employee, but only one can be marked as the highest degree

        builder.ConfigureAuditProperties();
    }
}