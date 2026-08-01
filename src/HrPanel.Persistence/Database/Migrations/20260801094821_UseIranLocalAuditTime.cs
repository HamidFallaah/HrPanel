using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrPanel.Persistence.Migrations
{
    public partial class UseIranLocalAuditTime : Migration
    {
        private const int IranOffsetMinutes = 210;

        private static readonly (string Schema, string Table)[]
            AuditableTables =
            [
                ("attendance", "WorkSchedules"),
                ("attendance", "WorkScheduleDays"),
                ("attendance", "Shifts"),
                ("attendance", "EmployeeScheduleAssignments"),

                ("org", "WorkLocations"),
                ("org", "Positions"),
                ("org", "OrganizationUnits"),
                ("org", "OperationalGroups"),

                ("hr", "ExternalPersons"),
                ("hr", "Employments"),
                ("hr", "Employees"),
                ("hr", "EmployeeRelationships"),
                ("hr", "EmployeeOperationalGroupAssignments"),
                ("hr", "EmployeeIdentifiers"),
                ("hr", "EmployeeEducations"),
                ("hr", "EmployeeDependents"),
                ("hr", "EmployeeContacts"),
                ("hr", "EmployeeAssignments"),
                ("hr", "DisciplinaryActions"),

                ("asset", "EmployeeAssetAssignments"),
                ("asset", "Assets")
            ];

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var (schema, table) in AuditableTables)
            {
                migrationBuilder.RenameColumn(
                    name: "ModifiedAtUtc",
                    schema: schema,
                    table: table,
                    newName: "ModifiedAt");

                migrationBuilder.RenameColumn(
                    name: "CreatedAtUtc",
                    schema: schema,
                    table: table,
                    newName: "CreatedAt");

                migrationBuilder.RenameIndex(
                    name: $"IX_{table}_CreatedAtUtc",
                    schema: schema,
                    table: table,
                    newName: $"IX_{table}_CreatedAt");
            }

            foreach (var (schema, table) in AuditableTables)
            {
                migrationBuilder.Sql(
                    $"""
                    UPDATE [{schema}].[{table}]
                    SET
                        [CreatedAt] =
                            DATEADD(
                                MINUTE,
                                {IranOffsetMinutes},
                                [CreatedAt]),
                        [ModifiedAt] =
                            DATEADD(
                                MINUTE,
                                {IranOffsetMinutes},
                                [ModifiedAt]);
                    """);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var (schema, table) in AuditableTables)
            {
                migrationBuilder.Sql(
                    $"""
                    UPDATE [{schema}].[{table}]
                    SET
                        [CreatedAt] =
                            DATEADD(
                                MINUTE,
                                -{IranOffsetMinutes},
                                [CreatedAt]),
                        [ModifiedAt] =
                            DATEADD(
                                MINUTE,
                                -{IranOffsetMinutes},
                                [ModifiedAt]);
                    """);
            }

            foreach (var (schema, table) in AuditableTables)
            {
                migrationBuilder.RenameColumn(
                    name: "ModifiedAt",
                    schema: schema,
                    table: table,
                    newName: "ModifiedAtUtc");

                migrationBuilder.RenameColumn(
                    name: "CreatedAt",
                    schema: schema,
                    table: table,
                    newName: "CreatedAtUtc");

                migrationBuilder.RenameIndex(
                    name: $"IX_{table}_CreatedAt",
                    schema: schema,
                    table: table,
                    newName: $"IX_{table}_CreatedAtUtc");
            }
        }
    }
}