using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrPanel.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeSchedulingPatterns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
        """
        IF EXISTS
        (
        SELECT 1
        FROM [attendance].[WorkSchedules]
        )
        OR EXISTS
        (
        SELECT 1
        FROM [attendance].[EmployeeScheduleAssignments]
        )
         BEGIN
        THROW 51000,'Scheduling normalization requires the scheduling tables to be empty.',1;END
        """);
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_Shifts_ShiftId",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_ShiftId",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkSchedules_WorkDaysPerWeek",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Shifts_DifferentStartAndEndTime",
                schema: "attendance",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "WorkDaysPerWeek",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.AddColumn<DateOnly>(
                name: "AnchorDate",
                schema: "attendance",
                table: "WorkSchedules",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CycleLengthDays",
                schema: "attendance",
                table: "WorkSchedules",
                type: "smallint",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                schema: "attendance",
                table: "WorkSchedules",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatternType",
                schema: "attendance",
                table: "WorkSchedules",
                type: "int",
                nullable: false);

            migrationBuilder.AddColumn<short>(
                name: "RotationOffsetDays",
                schema: "attendance",
                table: "EmployeeScheduleAssignments",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "WorkScheduleDays",
                schema: "attendance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    DayIndex = table.Column<short>(type: "smallint", nullable: false),
                    ShiftId = table.Column<long>(type: "bigint", nullable: true),
                    IsRestDay = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkScheduleDays", x => x.Id);
                    table.CheckConstraint("CK_WorkScheduleDays_DayIndex", "[DayIndex] >= 0 AND [DayIndex] <= 365");
                    table.CheckConstraint("CK_WorkScheduleDays_ShiftOrRestDay", "([IsRestDay] = 1 AND [ShiftId] IS NULL) OR ([IsRestDay] = 0 AND [ShiftId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_WorkScheduleDays_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalSchema: "attendance",
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkScheduleDays_WorkSchedules_WorkScheduleId",
                        column: x => x.WorkScheduleId,
                        principalSchema: "attendance",
                        principalTable: "WorkSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkSchedules_CycleLengthDays",
                schema: "attendance",
                table: "WorkSchedules",
                sql: "[CycleLengthDays] >= 1 AND [CycleLengthDays] <= 366");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkSchedules_PatternType",
                schema: "attendance",
                table: "WorkSchedules",
                sql: "[PatternType] IN (1, 2, 3)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkSchedules_RotatingAnchorDate",
                schema: "attendance",
                table: "WorkSchedules",
                sql: "[PatternType] <> 2 OR [AnchorDate] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_EmployeeScheduleAssignments_RotationOffsetDays",
                schema: "attendance",
                table: "EmployeeScheduleAssignments",
                sql: "[RotationOffsetDays] >= 0 AND [RotationOffsetDays] <= 365");

            migrationBuilder.CreateIndex(
                name: "IX_WorkScheduleDays_CreatedAtUtc",
                schema: "attendance",
                table: "WorkScheduleDays",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_WorkScheduleDays_ShiftId",
                schema: "attendance",
                table: "WorkScheduleDays",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "UX_WorkScheduleDays_Schedule_DayIndex",
                schema: "attendance",
                table: "WorkScheduleDays",
                columns: new[] { "WorkScheduleId", "DayIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            """
            IF EXISTS
            (
                SELECT 1
                FROM [attendance].[WorkSchedules]
            )
                BEGIN
                THROW 51001,'Remove or manually convert normalized work schedules before rolling back this migration.',1;
                END

            IF EXISTS
            (
                SELECT 1
                FROM [attendance].[Shifts]
                WHERE [StartTime] = [EndTime]
            )
                BEGIN THROW 51002,'A 24-hour shift exists. Remove or convert it before rolling back this migration.',1;END
            """);
            migrationBuilder.DropTable(
                name: "WorkScheduleDays",
                schema: "attendance");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkSchedules_CycleLengthDays",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkSchedules_PatternType",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkSchedules_RotatingAnchorDate",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_EmployeeScheduleAssignments_RotationOffsetDays",
                schema: "attendance",
                table: "EmployeeScheduleAssignments");

            migrationBuilder.DropColumn(
                name: "AnchorDate",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "CycleLengthDays",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "NameEn",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "PatternType",
                schema: "attendance",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "RotationOffsetDays",
                schema: "attendance",
                table: "EmployeeScheduleAssignments");

            migrationBuilder.AddColumn<long>(
                name: "ShiftId",
                schema: "attendance",
                table: "WorkSchedules",
                type: "bigint",
                nullable: false);

            migrationBuilder.AddColumn<decimal>(
                name: "WorkDaysPerWeek",
                schema: "attendance",
                table: "WorkSchedules",
                type: "decimal(3,1)",
                precision: 3,
                scale: 1,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_ShiftId",
                schema: "attendance",
                table: "WorkSchedules",
                column: "ShiftId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkSchedules_WorkDaysPerWeek",
                schema: "attendance",
                table: "WorkSchedules",
                sql: "[WorkDaysPerWeek] > 0 AND [WorkDaysPerWeek] <= 7");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Shifts_DifferentStartAndEndTime",
                schema: "attendance",
                table: "Shifts",
                sql: "[StartTime] <> [EndTime]");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_Shifts_ShiftId",
                schema: "attendance",
                table: "WorkSchedules",
                column: "ShiftId",
                principalSchema: "attendance",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
