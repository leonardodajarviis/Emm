using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Presen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTasks_CreatedAt",
                table: "OperationShiftTasks");

            migrationBuilder.DropIndex(
                name: "IX_CheckpointValues_CreatedAt",
                table: "OperationShiftTaskCheckpointValues");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTaskCheckpoints_CreatedAt",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftParameterReadings_CreatedAt",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftAssets_CreatedAt",
                table: "OperationShiftAssets");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ParameterBasedMaintenanceTriggers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ParameterBasedMaintenanceTriggers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OperationShiftTasks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OperationShiftTasks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OperationShiftTaskCheckpointValues");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OperationShiftTaskCheckpointValues");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OperationShiftAssets");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OperationShiftAssets");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MaintenancePlanJobStepDefinitions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MaintenancePlanJobStepDefinitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ParameterBasedMaintenanceTriggers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ParameterBasedMaintenanceTriggers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OperationShiftTaskStatusHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OperationShiftTaskStatusHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OperationShiftTasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OperationShiftTasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OperationShiftTaskCheckpointValues",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OperationShiftTaskCheckpointValues",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OperationShiftTaskCheckpoints",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OperationShiftTaskCheckpoints",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OperationShiftParameterReadings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OperationShiftParameterReadings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OperationShiftAssets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OperationShiftAssets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MaintenancePlanJobStepDefinitions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MaintenancePlanJobStepDefinitions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTasks_CreatedAt",
                table: "OperationShiftTasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointValues_CreatedAt",
                table: "OperationShiftTaskCheckpointValues",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskCheckpoints_CreatedAt",
                table: "OperationShiftTaskCheckpoints",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_CreatedAt",
                table: "OperationShiftParameterReadings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_CreatedAt",
                table: "OperationShiftAssets",
                column: "CreatedAt");
        }
    }
}
