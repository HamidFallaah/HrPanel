using HrPanel.Domain.Assets;
using HrPanel.Domain.Employees;
using HrPanel.Domain.Organization;
using HrPanel.Domain.Scheduling;
using HrPanel.Persistence.Database.Seeds;
using HrPanel.Persistence.Identity;
using HrPanel.Persistence.LegacyImport;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DisciplinaryActionEntity = HrPanel.Domain.Employment.DisciplinaryAction;
using EmployeeAssignmentEntity = HrPanel.Domain.Employment.EmployeeAssignment;
using EmployeeRelationshipEntity = HrPanel.Domain.Employment.EmployeeRelationship;
using EmploymentEntity = HrPanel.Domain.Employment.Employment;
using EmploymentStatusEntity = HrPanel.Domain.Employment.EmploymentStatus;
using EmploymentTypeEntity = HrPanel.Domain.Employment.EmploymentType;
using ExternalPersonEntity = HrPanel.Domain.Employment.ExternalPerson;
using WorkTimeTypeEntity =HrPanel.Domain.Employment.WorkTimeType;
using EmployeeOperationalGroupAssignmentEntity = HrPanel.Domain.Employment.EmployeeOperationalGroupAssignment;
namespace HrPanel.Persistence.Database;

public sealed class HrDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public HrDbContext(DbContextOptions<HrDbContext> options): base(options)
    {

    }

    // Employees
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeePersonalDetails> EmployeePersonalDetails => Set<EmployeePersonalDetails>();
    public DbSet<EmployeeContact> EmployeeContacts => Set<EmployeeContact>();
    public DbSet<EmployeeIdentifier> EmployeeIdentifiers => Set<EmployeeIdentifier>();
    public DbSet<EmployeeDependent> EmployeeDependents => Set<EmployeeDependent>();
    public DbSet<EmployeeEducation> EmployeeEducations => Set<EmployeeEducation>();

    // Employment
    public DbSet<EmploymentEntity> Employments => Set<EmploymentEntity>();
    public DbSet<EmploymentTypeEntity> EmploymentTypes => Set<EmploymentTypeEntity>();
    public DbSet<EmploymentStatusEntity> EmploymentStatuses => Set<EmploymentStatusEntity>();
    public DbSet<EmployeeAssignmentEntity> EmployeeAssignments => Set<EmployeeAssignmentEntity>();
    public DbSet<EmployeeRelationshipEntity> EmployeeRelationships => Set<EmployeeRelationshipEntity>();
    public DbSet<ExternalPersonEntity> ExternalPersons => Set<ExternalPersonEntity>();
    public DbSet<DisciplinaryActionEntity> DisciplinaryActions => Set<DisciplinaryActionEntity>();
    public DbSet<WorkTimeTypeEntity> WorkTimeTypes => Set<WorkTimeTypeEntity>();
    public DbSet<EmployeeOperationalGroupAssignmentEntity> EmployeeOperationalGroupAssignments => Set<EmployeeOperationalGroupAssignmentEntity>();
    // Organization
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();
    public DbSet<OrganizationUnitType> OrganizationUnitTypes => Set<OrganizationUnitType>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<JobLevel> JobLevels => Set<JobLevel>();
    public DbSet<WorkLocation> WorkLocations => Set<WorkLocation>();
    public DbSet<OperationalGroup> OperationalGroups => Set<OperationalGroup>();

    // Scheduling
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<WorkSchedule> WorkSchedules => Set<WorkSchedule>();
    public DbSet<WorkScheduleDay> WorkScheduleDays => Set<WorkScheduleDay>();
    public DbSet<EmployeeScheduleAssignment>EmployeeScheduleAssignments => Set<EmployeeScheduleAssignment>();

    // Assets
    public DbSet<AssetType> AssetTypes => Set<AssetType>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<EmployeeAssetAssignment>EmployeeAssetAssignments => Set<EmployeeAssetAssignment>();

    // Legacy import
    public DbSet<LegacyEmployeeImportRow>LegacyEmployeeImportRows => Set<LegacyEmployeeImportRow>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseSchemas.Hr);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrDbContext).Assembly);

        IdentityModelConfiguration.Configure(modelBuilder);

        ReferenceDataSeeder.Seed(modelBuilder);
    }
}
