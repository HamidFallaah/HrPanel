using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrPanel.Persistence.Database.Migrations
{
    /// <inheritdoc />
    public partial class HardenLegacyImportAndEmployeeRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePersonalDetails_Employees_EmployeeId",
                schema: "hr",
                table: "EmployeePersonalDetails");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivedAtUtc",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedAtUtc",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                type: "datetime2(7)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_LegacyEmployeeImportRows_ImportStatus",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                sql: "[ImportStatus] IN (1, 2, 3, 4, 5)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LegacyEmployeeImportRows_SourceRowNumber",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                sql: "[SourceRowNumber] > 0");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePersonalDetails_Employees",
                schema: "hr",
                table: "EmployeePersonalDetails",
                column: "EmployeeId",
                principalSchema: "hr",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LegacyEmployeeImportRows_Employees",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                column: "ImportedEmployeeId",
                principalSchema: "hr",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePersonalDetails_Employees",
                schema: "hr",
                table: "EmployeePersonalDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_LegacyEmployeeImportRows_Employees",
                schema: "staging",
                table: "LegacyEmployeeImportRows");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LegacyEmployeeImportRows_ImportStatus",
                schema: "staging",
                table: "LegacyEmployeeImportRows");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LegacyEmployeeImportRows_SourceRowNumber",
                schema: "staging",
                table: "LegacyEmployeeImportRows");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivedAtUtc",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedAtUtc",
                schema: "staging",
                table: "LegacyEmployeeImportRows",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePersonalDetails_Employees_EmployeeId",
                schema: "hr",
                table: "EmployeePersonalDetails",
                column: "EmployeeId",
                principalSchema: "hr",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
