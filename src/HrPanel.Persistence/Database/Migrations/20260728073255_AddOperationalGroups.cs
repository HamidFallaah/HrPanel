using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrPanel.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationalGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationalGroups",
                schema: "org",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationalGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeOperationalGroupAssignments",
                schema: "hr",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false),
                    OperationalGroupId = table.Column<long>(type: "bigint", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeOperationalGroupAssignments", x => x.Id);
                    table.CheckConstraint("CK_EmployeeOperationalGroupAssignments_DateRange", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
                    table.ForeignKey(
                        name: "FK_EmployeeOperationalGroupAssignments_Employments_EmploymentId",
                        column: x => x.EmploymentId,
                        principalSchema: "hr",
                        principalTable: "Employments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeOperationalGroupAssignments_OperationalGroups_OperationalGroupId",
                        column: x => x.OperationalGroupId,
                        principalSchema: "org",
                        principalTable: "OperationalGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOperationalGroupAssignments_CreatedAtUtc",
                schema: "hr",
                table: "EmployeeOperationalGroupAssignments",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOperationalGroupAssignments_OperationalGroupId",
                schema: "hr",
                table: "EmployeeOperationalGroupAssignments",
                column: "OperationalGroupId");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeOperationalGroupAssignments_CurrentMembership",
                schema: "hr",
                table: "EmployeeOperationalGroupAssignments",
                columns: new[] { "EmploymentId", "OperationalGroupId" },
                unique: true,
                filter: "[EffectiveTo] IS NULL");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeOperationalGroupAssignments_CurrentPrimary",
                schema: "hr",
                table: "EmployeeOperationalGroupAssignments",
                column: "EmploymentId",
                unique: true,
                filter: "[EffectiveTo] IS NULL AND [IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_EmployeeOperationalGroupAssignments_Membership",
                schema: "hr",
                table: "EmployeeOperationalGroupAssignments",
                columns: new[] { "EmploymentId", "OperationalGroupId", "EffectiveFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationalGroups_CreatedAtUtc",
                schema: "org",
                table: "OperationalGroups",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_OperationalGroups_Type_IsActive",
                schema: "org",
                table: "OperationalGroups",
                columns: new[] { "Type", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "UX_OperationalGroups_Code",
                schema: "org",
                table: "OperationalGroups",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeOperationalGroupAssignments",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "OperationalGroups",
                schema: "org");
        }
    }
}
