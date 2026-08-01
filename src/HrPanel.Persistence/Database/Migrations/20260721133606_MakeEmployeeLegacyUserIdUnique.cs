using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrPanel.Persistence.Database.Migrations
{
    /// <inheritdoc />
    public partial class MakeEmployeeLegacyUserIdUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_LegacyUserId",
                schema: "hr",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "UX_Employees_LegacyUserId",
                schema: "hr",
                table: "Employees",
                column: "LegacyUserId",
                unique: true,
                filter: "[LegacyUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Employees_LegacyUserId",
                schema: "hr",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LegacyUserId",
                schema: "hr",
                table: "Employees",
                column: "LegacyUserId",
                filter: "[LegacyUserId] IS NOT NULL");
        }
    }
}
