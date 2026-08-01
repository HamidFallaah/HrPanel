using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrPanel.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkTimeTypesToEmployments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "WorkTimeTypeId",
                schema: "hr",
                table: "Employments",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkTimeTypes",
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
                    table.PrimaryKey("PK_WorkTimeTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "hr",
                table: "WorkTimeTypes",
                columns: new[] { "Id", "Code", "IsActive", "NameEn", "NameFa" },
                values: new object[,]
                {
                    { (short)1, "FULL_TIME", true, "Full Time", "تمام‌ وقت" },
                    { (short)2, "PART_TIME", true, "Part Time", "پاره ‌وقت" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employments_WorkTimeTypeId",
                schema: "hr",
                table: "Employments",
                column: "WorkTimeTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_WorkTimeTypes_Code",
                schema: "hr",
                table: "WorkTimeTypes",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_WorkTimeTypes_WorkTimeTypeId",
                schema: "hr",
                table: "Employments",
                column: "WorkTimeTypeId",
                principalSchema: "hr",
                principalTable: "WorkTimeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_WorkTimeTypes_WorkTimeTypeId",
                schema: "hr",
                table: "Employments");

            migrationBuilder.DropTable(
                name: "WorkTimeTypes",
                schema: "hr");

            migrationBuilder.DropIndex(
                name: "IX_Employments_WorkTimeTypeId",
                schema: "hr",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "WorkTimeTypeId",
                schema: "hr",
                table: "Employments");
        }
    }
}
