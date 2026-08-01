using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrPanel.Persistence.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLegacyEmploymentLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "hr",
                table: "EmploymentStatuses",
                columns: new[] { "Id", "Code", "IsActive", "NameEn", "NameFa" },
                values: new object[,]
                {
                    { (short)5, "MATERNITY_LEAVE", true, "Maternity Leave", "مرخصی زایمان" },
                    { (short)6, "TRANSFERRED", true, "Transferred", "انتقال ‌یافته" }
                });

            migrationBuilder.InsertData(
                schema: "hr",
                table: "EmploymentTypes",
                columns: new[] { "Id", "Code", "IsActive", "NameEn", "NameFa" },
                values: new object[] { (short)5, "LOCAL", true, "Local", "نیروی داخلی" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "hr",
                table: "EmploymentStatuses",
                keyColumn: "Id",
                keyValue: (short)5);

            migrationBuilder.DeleteData(
                schema: "hr",
                table: "EmploymentStatuses",
                keyColumn: "Id",
                keyValue: (short)6);

            migrationBuilder.DeleteData(
                schema: "hr",
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: (short)5);
        }
    }
}
