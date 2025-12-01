using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BB01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeBasedMaintenanceTriggers");

            // migrationBuilder.AlterColumn<byte[]>(
            //     name: "RowVersion",
            //     table: "OutboxMessages",
            //     type: "rowversion",
            //     rowVersion: true,
            //     nullable: false,
            //     defaultValue: new byte[0],
            //     oldClrType: typeof(byte[]),
            //     oldType: "rowversion",
            //     oldRowVersion: true,
            //     oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaintenancePlanInstances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaintenancePlanDefinitions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RRule",
                table: "MaintenancePlanDefinitions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RRule",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "OutboxMessages",
                type: "rowversion",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaintenancePlanInstances",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MaintenancePlanDefinitions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateTable(
                name: "TimeBasedMaintenanceTriggers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DayOfMonth = table.Column<int>(type: "int", nullable: true),
                    DayOfWeek = table.Column<int>(type: "int", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HourOfDay = table.Column<int>(type: "int", nullable: true),
                    IntervalValue = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaintenancePlanDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    MonthOfYear = table.Column<int>(type: "int", nullable: true),
                    ScheduleType = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeBasedMaintenanceTriggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeBasedMaintenanceTriggers_MaintenancePlanDefinitions_MaintenancePlanDefinitionId",
                        column: x => x.MaintenancePlanDefinitionId,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggers_IsActive",
                table: "TimeBasedMaintenanceTriggers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggers_MaintenancePlanDefinitionId",
                table: "TimeBasedMaintenanceTriggers",
                column: "MaintenancePlanDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggers_ScheduleType",
                table: "TimeBasedMaintenanceTriggers",
                column: "ScheduleType");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggers_StartDate_EndDate",
                table: "TimeBasedMaintenanceTriggers",
                columns: new[] { "StartDate", "EndDate" });
        }
    }
}
