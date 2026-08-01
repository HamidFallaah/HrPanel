using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrPanel.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrectJobLevelSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)1,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "1", "Level 1", "سطح ۱" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)2,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "1H", "Level 1H", "سطح ۱H" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)3,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "2", "Level 2", "سطح ۲" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)4,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "2H", "Level 2H", "سطح ۲H" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)5,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "3", "Level 3", "سطح ۳" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)6,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "3H", "Level 3H", "سطح ۳H" });

            migrationBuilder.InsertData(
                schema: "org",
                table: "JobLevels",
                columns: new[] { "Id", "Code", "IsActive", "Rank", "TitleEn", "TitleFa" },
                values: new object[] { (short)7, "4", true, (short)7, "Level 4", "سطح ۴" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)7);

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)1,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "INTERN", "Intern", "کارآموز" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)2,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "JUNIOR", "Junior", "کارشناس تازه‌کار" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)3,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "MID", "Mid-Level", "کارشناس" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)4,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "SENIOR", "Senior", "کارشناس ارشد" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)5,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "LEAD", "Lead", "سرپرست" });

            migrationBuilder.UpdateData(
                schema: "org",
                table: "JobLevels",
                keyColumn: "Id",
                keyValue: (short)6,
                columns: new[] { "Code", "TitleEn", "TitleFa" },
                values: new object[] { "MANAGER", "Manager", "مدیر" });
        }
    }
}
