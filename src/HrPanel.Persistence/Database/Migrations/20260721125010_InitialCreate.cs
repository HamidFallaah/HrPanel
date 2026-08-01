using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrPanel.Persistence.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "asset");

            migrationBuilder.EnsureSchema(
                name: "hr");

            migrationBuilder.EnsureSchema(
                name: "attendance");

            migrationBuilder.EnsureSchema(
                name: "org");

            migrationBuilder.EnsureSchema(
                name: "staging");

            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                schema: "asset",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NameFa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LegacyUserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LegacyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.CheckConstraint("CK_Employees_EmployeeNumber_NotEmpty", "LEN(LTRIM(RTRIM([EmployeeNumber]))) > 0");
                });

            migrationBuilder.CreateTable(
                name: "EmploymentStatuses",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NameFa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentTypes",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NameFa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalPersons",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LegacyUsername = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalPersons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobLevels",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TitleFa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Rank = table.Column<short>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobLevels", x => x.Id);
                    table.CheckConstraint("CK_JobLevels_Rank", "[Rank] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "LegacyEmployeeImportRows",
                schema: "staging",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceRowNumber = table.Column<int>(type: "int", nullable: false),
                    ImportStatus = table.Column<short>(type: "smallint", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportedEmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    ErrorDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstNameFa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastNameFa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmploymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmploymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupervisorUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QaUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartWork = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndWork = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Division = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pilot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionHr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionCr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArchiveNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FoodCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelephoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternateEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShiftType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmitBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmitDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeniorManager = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubDivision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Education = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerUsername2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerUsername3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerUsername4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DivisionCr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupervisorUsernameCr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerUsernameCr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerUsernameCr2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerUsernameCr3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerUsernameCr4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartWorkFirst = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractTerm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarningStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarningEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarningDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherNationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Marital = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpouseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpouseNationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvinceWork = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityWork = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Td = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Imei = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegacyId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegacyEmployeeImportRows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUnitTypes",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NameFa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HierarchyOrder = table.Column<short>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUnitTypes", x => x.Id);
                    table.CheckConstraint("CK_OrganizationUnitTypes_HierarchyOrder", "[HierarchyOrder] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TitleFa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                schema: "attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NameFa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    WorkHours = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                    table.CheckConstraint("CK_Shifts_DifferentStartAndEndTime", "[StartTime] <> [EndTime]");
                    table.CheckConstraint("CK_Shifts_WorkHours", "[WorkHours] > 0 AND [WorkHours] <= 24");
                });

            migrationBuilder.CreateTable(
                name: "WorkLocations",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NameFa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "asset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetTypeId = table.Column<short>(type: "smallint", nullable: false),
                    AssetTag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ServiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Imei = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalSchema: "asset",
                        principalTable: "AssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DisciplinaryActions",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplinaryActions", x => x.Id);
                    table.CheckConstraint("CK_DisciplinaryActions_DateRange", "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
                    table.ForeignKey(
                        name: "FK_DisciplinaryActions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeContacts",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeContacts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDependents",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NationalCode = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RelationshipType = table.Column<short>(type: "smallint", nullable: false),
                    IsEmergencyContact = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EmergencyPhone = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDependents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDependents_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeEducations",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    DegreeTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    InstitutionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GraduationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsHighestDegree = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeEducations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeIdentifiers",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeIdentifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeIdentifiers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePersonalDetails",
                schema: "hr",
                columns: table => new
                {
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FirstNameFa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastNameFa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NationalCode = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FatherNationalCode = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    BirthPlace = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Gender = table.Column<short>(type: "smallint", nullable: false),
                    MaritalStatus = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePersonalDetails", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_EmployeePersonalDetails_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Employments",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    EmploymentTypeId = table.Column<short>(type: "smallint", nullable: false),
                    EmploymentStatusId = table.Column<short>(type: "smallint", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ContractTermMonths = table.Column<short>(type: "smallint", nullable: true),
                    TerminationReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employments", x => x.Id);
                    table.CheckConstraint("CK_Employments_ContractTermMonths", "[ContractTermMonths] IS NULL OR ([ContractTermMonths] >= 1 AND [ContractTermMonths] <= 120)");
                    table.CheckConstraint("CK_Employments_DateRange", "[EndDate] IS NULL OR [EndDate] >= [StartDate]");
                    table.ForeignKey(
                        name: "FK_Employments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employments_EmploymentStatuses_EmploymentStatusId",
                        column: x => x.EmploymentStatusId,
                        principalSchema: "hr",
                        principalTable: "EmploymentStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employments_EmploymentTypes_EmploymentTypeId",
                        column: x => x.EmploymentTypeId,
                        principalSchema: "hr",
                        principalTable: "EmploymentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRelationships",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Context = table.Column<short>(type: "smallint", nullable: false),
                    RelatedEmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedExternalPersonId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRelationships", x => x.Id);
                    table.CheckConstraint("CK_EmployeeRelationships_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.CheckConstraint("CK_EmployeeRelationships_NotSelf", "[RelatedEmployeeId] IS NULL OR [RelatedEmployeeId] <> [EmployeeId]");
                    table.CheckConstraint("CK_EmployeeRelationships_SingleRelatedParty", "([RelatedEmployeeId] IS NOT NULL AND [RelatedExternalPersonId] IS NULL) OR ([RelatedEmployeeId] IS NULL AND [RelatedExternalPersonId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_EmployeeRelationships_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeRelationships_Employees_RelatedEmployeeId",
                        column: x => x.RelatedEmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeRelationships_ExternalPersons_RelatedExternalPersonId",
                        column: x => x.RelatedExternalPersonId,
                        principalSchema: "hr",
                        principalTable: "ExternalPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUnits",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationUnitTypeId = table.Column<short>(type: "smallint", nullable: false),
                    ParentOrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NameFa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUnits", x => x.Id);
                    table.CheckConstraint("CK_OrganizationUnits_NotOwnParent", "[ParentOrganizationUnitId] IS NULL OR [ParentOrganizationUnitId] <> [Id]");
                    table.ForeignKey(
                        name: "FK_OrganizationUnits_OrganizationUnitTypes_OrganizationUnitTypeId",
                        column: x => x.OrganizationUnitTypeId,
                        principalSchema: "org",
                        principalTable: "OrganizationUnitTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationUnits_OrganizationUnits_ParentOrganizationUnitId",
                        column: x => x.ParentOrganizationUnitId,
                        principalSchema: "org",
                        principalTable: "OrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkSchedules",
                schema: "attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NameFa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ShiftId = table.Column<long>(type: "bigint", nullable: false),
                    WorkDaysPerWeek = table.Column<decimal>(type: "decimal(3,1)", precision: 3, scale: 1, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSchedules", x => x.Id);
                    table.CheckConstraint("CK_WorkSchedules_WorkDaysPerWeek", "[WorkDaysPerWeek] > 0 AND [WorkDaysPerWeek] <= 7");
                    table.ForeignKey(
                        name: "FK_WorkSchedules_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalSchema: "attendance",
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAssetAssignments",
                schema: "asset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    ReturnedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAssetAssignments", x => x.Id);
                    table.CheckConstraint("CK_EmployeeAssetAssignments_DateRange", "[ReturnedAt] IS NULL OR [ReturnedAt] >= [AssignedAt]");
                    table.ForeignKey(
                        name: "FK_EmployeeAssetAssignments_Assets_AssetId",
                        column: x => x.AssetId,
                        principalSchema: "asset",
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAssetAssignments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "hr",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAssignments",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    Context = table.Column<short>(type: "smallint", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    PositionId = table.Column<long>(type: "bigint", nullable: true),
                    JobLevelId = table.Column<short>(type: "smallint", nullable: true),
                    WorkLocationId = table.Column<long>(type: "bigint", nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAssignments", x => x.Id);
                    table.CheckConstraint("CK_EmployeeAssignments_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.ForeignKey(
                        name: "FK_EmployeeAssignments_Employments_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "hr",
                        principalTable: "Employments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeAssignments_JobLevels_JobLevelId",
                        column: x => x.JobLevelId,
                        principalSchema: "org",
                        principalTable: "JobLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAssignments_OrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalSchema: "org",
                        principalTable: "OrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAssignments_Positions_PositionId",
                        column: x => x.PositionId,
                        principalSchema: "org",
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAssignments_WorkLocations_WorkLocationId",
                        column: x => x.WorkLocationId,
                        principalSchema: "org",
                        principalTable: "WorkLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeScheduleAssignments",
                schema: "attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    WorkScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeScheduleAssignments", x => x.Id);
                    table.CheckConstraint("CK_EmployeeScheduleAssignments_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleAssignments_Employments_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "hr",
                        principalTable: "Employments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleAssignments_WorkSchedules_WorkScheduleId",
                        column: x => x.WorkScheduleId,
                        principalSchema: "attendance",
                        principalTable: "WorkSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "asset",
                table: "AssetTypes",
                columns: new[] { "Id", "Code", "IsActive", "NameEn", "NameFa" },
                values: new object[,]
                {
                    { (short)1, "TDLTE_MODEM", true, "TD-LTE Modem", "مودم TD-LTE" },
                    { (short)2, "SIM_CARD", true, "SIM Card", "سیم‌کارت" },
                    { (short)3, "LAPTOP", true, "Laptop", "لپ‌تاپ" },
                    { (short)4, "ACCESS_CARD", true, "Access Card", "کارت تردد" },
                    { (short)5, "MOBILE_PHONE", true, "Mobile Phone", "تلفن همراه" },
                    { (short)6, "OTHER", true, "Other", "سایر" }
                });

            migrationBuilder.InsertData(
                schema: "hr",
                table: "EmploymentStatuses",
                columns: new[] { "Id", "Code", "IsActive", "NameEn", "NameFa" },
                values: new object[,]
                {
                    { (short)1, "ACTIVE", true, "Active", "فعال" },
                    { (short)2, "INACTIVE", true, "Inactive", "غیرفعال" },
                    { (short)3, "TERMINATED", true, "Terminated", "خاتمه همکاری" },
                    { (short)4, "RESIGNED", true, "Resigned", "استعفا" }
                });

            migrationBuilder.InsertData(
                schema: "hr",
                table: "EmploymentTypes",
                columns: new[] { "Id", "Code", "IsActive", "NameEn", "NameFa" },
                values: new object[,]
                {
                    { (short)1, "PERMANENT", true, "Permanent", "رسمی" },
                    { (short)2, "CONTRACT", true, "Contract", "قراردادی" },
                    { (short)3, "VENDOR", true, "Vendor", "پیمانکاری" },
                    { (short)4, "PROJECT", true, "Project", "پروژه‌ای" }
                });

            migrationBuilder.InsertData(
                schema: "org",
                table: "JobLevels",
                columns: new[] { "Id", "Code", "IsActive", "Rank", "TitleEn", "TitleFa" },
                values: new object[,]
                {
                    { (short)1, "INTERN", true, (short)1, "Intern", "کارآموز" },
                    { (short)2, "JUNIOR", true, (short)2, "Junior", "کارشناس تازه‌کار" },
                    { (short)3, "MID", true, (short)3, "Mid-Level", "کارشناس" },
                    { (short)4, "SENIOR", true, (short)4, "Senior", "کارشناس ارشد" },
                    { (short)5, "LEAD", true, (short)5, "Lead", "سرپرست" },
                    { (short)6, "MANAGER", true, (short)6, "Manager", "مدیر" }
                });

            migrationBuilder.InsertData(
                schema: "org",
                table: "OrganizationUnitTypes",
                columns: new[] { "Id", "Code", "HierarchyOrder", "IsActive", "NameEn", "NameFa" },
                values: new object[,]
                {
                    { (short)1, "COMPANY", (short)1, true, "Company", "شرکت" },
                    { (short)2, "DIVISION", (short)2, true, "Division", "معاونت" },
                    { (short)3, "SUBDIVISION", (short)3, true, "Subdivision", "زیرمعاونت" },
                    { (short)4, "DEPARTMENT", (short)4, true, "Department", "اداره" },
                    { (short)5, "SECTION", (short)5, true, "Section", "بخش" },
                    { (short)6, "UNIT", (short)6, true, "Unit", "واحد" }
                });

            migrationBuilder.InsertData(
                schema: "identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0a40cd90-4bb6-4f98-8ce3-082ecfcb1bcb"), "1AA90549-3B27-4015-9CC8-CBF6B931671A", "HrStaff", "HRSTAFF" },
                    { new Guid("e8137cf2-5b8b-4c4e-9eaa-899a0430476a"), "7A418004-C400-4B97-AAB4-9A9D7789C114", "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetTypeId",
                schema: "asset",
                table: "Assets",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CreatedAtUtc",
                schema: "asset",
                table: "Assets",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "UX_Assets_AssetTag",
                schema: "asset",
                table: "Assets",
                column: "AssetTag",
                unique: true,
                filter: "[AssetTag] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Assets_Imei",
                schema: "asset",
                table: "Assets",
                column: "Imei",
                unique: true,
                filter: "[Imei] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Assets_SerialNumber",
                schema: "asset",
                table: "Assets",
                column: "SerialNumber",
                unique: true,
                filter: "[SerialNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Assets_ServiceNumber",
                schema: "asset",
                table: "Assets",
                column: "ServiceNumber",
                unique: true,
                filter: "[ServiceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_AssetTypes_Code",
                schema: "asset",
                table: "AssetTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinaryActions_CreatedAtUtc",
                schema: "hr",
                table: "DisciplinaryActions",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_DisciplinaryActions_Employee_Date",
                schema: "hr",
                table: "DisciplinaryActions",
                columns: new[] { "EmployeeId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAssetAssignments_CreatedAtUtc",
                schema: "asset",
                table: "EmployeeAssetAssignments",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAssetAssignments_EmployeeId",
                schema: "asset",
                table: "EmployeeAssetAssignments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeAssetAssignments_CurrentAsset",
                schema: "asset",
                table: "EmployeeAssetAssignments",
                column: "AssetId",
                unique: true,
                filter: "[ReturnedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAssignments_CreatedAtUtc",
                schema: "hr",
                table: "EmployeeAssignments",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAssignments_JobLevelId",
                schema: "hr",
                table: "EmployeeAssignments",
                column: "JobLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAssignments_OrganizationUnitId",
                schema: "hr",
                table: "EmployeeAssignments",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAssignments_PositionId",
                schema: "hr",
                table: "EmployeeAssignments",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAssignments_WorkLocationId",
                schema: "hr",
                table: "EmployeeAssignments",
                column: "WorkLocationId");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeAssignments_CurrentByContext",
                schema: "hr",
                table: "EmployeeAssignments",
                columns: new[] { "EmploymentId", "Context" },
                unique: true,
                filter: "[EffectiveTo] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContacts_CreatedAtUtc",
                schema: "hr",
                table: "EmployeeContacts",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContacts_EmployeeId",
                schema: "hr",
                table: "EmployeeContacts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeContacts_PrimaryByType",
                schema: "hr",
                table: "EmployeeContacts",
                columns: new[] { "EmployeeId", "Type" },
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDependents_CreatedAtUtc",
                schema: "hr",
                table: "EmployeeDependents",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDependents_EmployeeId",
                schema: "hr",
                table: "EmployeeDependents",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEducations_CreatedAtUtc",
                schema: "hr",
                table: "EmployeeEducations",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeEducations_HighestDegree",
                schema: "hr",
                table: "EmployeeEducations",
                column: "EmployeeId",
                unique: true,
                filter: "[IsHighestDegree] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeIdentifiers_CreatedAtUtc",
                schema: "hr",
                table: "EmployeeIdentifiers",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeIdentifiers_EmployeeId",
                schema: "hr",
                table: "EmployeeIdentifiers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeIdentifiers_ActiveValue",
                schema: "hr",
                table: "EmployeeIdentifiers",
                columns: new[] { "Type", "Value" },
                unique: true,
                filter: "[EffectiveTo] IS NULL");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeePersonalDetails_NationalCode",
                schema: "hr",
                table: "EmployeePersonalDetails",
                column: "NationalCode",
                unique: true,
                filter: "[NationalCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRelationships_CreatedAtUtc",
                schema: "hr",
                table: "EmployeeRelationships",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRelationships_RelatedEmployeeId",
                schema: "hr",
                table: "EmployeeRelationships",
                column: "RelatedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRelationships_RelatedExternalPersonId",
                schema: "hr",
                table: "EmployeeRelationships",
                column: "RelatedExternalPersonId");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeRelationships_CurrentByContext",
                schema: "hr",
                table: "EmployeeRelationships",
                columns: new[] { "EmployeeId", "Type", "Context" },
                unique: true,
                filter: "[EffectiveTo] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CreatedAtUtc",
                schema: "hr",
                table: "Employees",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LegacyUserId",
                schema: "hr",
                table: "Employees",
                column: "LegacyUserId",
                filter: "[LegacyUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Employees_EmployeeNumber",
                schema: "hr",
                table: "Employees",
                column: "EmployeeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Employees_LegacyGuid",
                schema: "hr",
                table: "Employees",
                column: "LegacyGuid",
                unique: true,
                filter: "[LegacyGuid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleAssignments_CreatedAtUtc",
                schema: "attendance",
                table: "EmployeeScheduleAssignments",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleAssignments_Employment_Date",
                schema: "attendance",
                table: "EmployeeScheduleAssignments",
                columns: new[] { "EmploymentId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleAssignments_WorkScheduleId",
                schema: "attendance",
                table: "EmployeeScheduleAssignments",
                column: "WorkScheduleId");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeScheduleAssignments_CurrentEmployment",
                schema: "attendance",
                table: "EmployeeScheduleAssignments",
                column: "EmploymentId",
                unique: true,
                filter: "[EffectiveTo] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_CreatedAtUtc",
                schema: "hr",
                table: "Employments",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_EmploymentStatusId",
                schema: "hr",
                table: "Employments",
                column: "EmploymentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_EmploymentTypeId",
                schema: "hr",
                table: "Employments",
                column: "EmploymentTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_Employments_CurrentByEmployee",
                schema: "hr",
                table: "Employments",
                column: "EmployeeId",
                unique: true,
                filter: "[EndDate] IS NULL");

            migrationBuilder.CreateIndex(
                name: "UX_EmploymentStatuses_Code",
                schema: "hr",
                table: "EmploymentStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_EmploymentTypes_Code",
                schema: "hr",
                table: "EmploymentTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPersons_CreatedAtUtc",
                schema: "hr",
                table: "ExternalPersons",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "UX_ExternalPersons_LegacyUsername",
                schema: "hr",
                table: "ExternalPersons",
                column: "LegacyUsername",
                unique: true,
                filter: "[LegacyUsername] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JobLevels_Rank",
                schema: "org",
                table: "JobLevels",
                column: "Rank");

            migrationBuilder.CreateIndex(
                name: "UX_JobLevels_Code",
                schema: "org",
                table: "JobLevels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LegacyEmployeeImportRows_ImportedEmployeeId",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                column: "ImportedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LegacyEmployeeImportRows_Status",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                column: "ImportStatus");

            migrationBuilder.CreateIndex(
                name: "UX_LegacyEmployeeImportRows_Batch_Row",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                columns: new[] { "BatchId", "SourceRowNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnits_CreatedAtUtc",
                schema: "org",
                table: "OrganizationUnits",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnits_ParentId",
                schema: "org",
                table: "OrganizationUnits",
                column: "ParentOrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnits_TypeId",
                schema: "org",
                table: "OrganizationUnits",
                column: "OrganizationUnitTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_OrganizationUnits_Code",
                schema: "org",
                table: "OrganizationUnits",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnitTypes_HierarchyOrder",
                schema: "org",
                table: "OrganizationUnitTypes",
                column: "HierarchyOrder");

            migrationBuilder.CreateIndex(
                name: "UX_OrganizationUnitTypes_Code",
                schema: "org",
                table: "OrganizationUnitTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CreatedAtUtc",
                schema: "org",
                table: "Positions",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "UX_Positions_Code",
                schema: "org",
                table: "Positions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "identity",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_CreatedAtUtc",
                schema: "attendance",
                table: "Shifts",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "UX_Shifts_Code",
                schema: "attendance",
                table: "Shifts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "identity",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "identity",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Users_EmployeeId",
                schema: "identity",
                table: "Users",
                column: "EmployeeId",
                unique: true,
                filter: "[EmployeeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLocations_CreatedAtUtc",
                schema: "org",
                table: "WorkLocations",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLocations_Province_City",
                schema: "org",
                table: "WorkLocations",
                columns: new[] { "Province", "City" });

            migrationBuilder.CreateIndex(
                name: "UX_WorkLocations_Code",
                schema: "org",
                table: "WorkLocations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_CreatedAtUtc",
                schema: "attendance",
                table: "WorkSchedules",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_ShiftId",
                schema: "attendance",
                table: "WorkSchedules",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "UX_WorkSchedules_Code",
                schema: "attendance",
                table: "WorkSchedules",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisciplinaryActions",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmployeeAssetAssignments",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "EmployeeAssignments",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmployeeContacts",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmployeeDependents",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmployeeEducations",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmployeeIdentifiers",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmployeePersonalDetails",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmployeeRelationships",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmployeeScheduleAssignments",
                schema: "attendance");

            migrationBuilder.DropTable(
                name: "LegacyEmployeeImportRows",
                schema: "staging");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "JobLevels",
                schema: "org");

            migrationBuilder.DropTable(
                name: "OrganizationUnits",
                schema: "org");

            migrationBuilder.DropTable(
                name: "Positions",
                schema: "org");

            migrationBuilder.DropTable(
                name: "WorkLocations",
                schema: "org");

            migrationBuilder.DropTable(
                name: "ExternalPersons",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Employments",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "WorkSchedules",
                schema: "attendance");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AssetTypes",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "OrganizationUnitTypes",
                schema: "org");

            migrationBuilder.DropTable(
                name: "EmploymentStatuses",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "EmploymentTypes",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "Shifts",
                schema: "attendance");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "hr");
        }
    }
}
