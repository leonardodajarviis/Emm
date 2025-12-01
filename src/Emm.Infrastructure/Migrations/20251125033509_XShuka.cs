using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class XShuka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationShiftTasks_OperationShifts_OperationShiftId",
                table: "OperationShiftTasks");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTaskStatusHistory_Severity",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTasks_Status",
                table: "OperationShiftTasks");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTasks_Type",
                table: "OperationShiftTasks");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftParameterReadings_Status",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftParameterReadings_Type",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OperationShiftTasks");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "OperationShiftTasks");

            migrationBuilder.DropColumn(
                name: "ReadBy",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "OperationShiftParameterReadings");

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

            migrationBuilder.AddColumn<long>(
                name: "AssetTypeId",
                table: "Assets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OperationShiftTasks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OperationShiftTasks");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "OperationShiftTaskStatusHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OperationShiftTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "OperationShiftTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReadBy",
                table: "OperationShiftParameterReadings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OperationShiftParameterReadings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "OperationShiftParameterReadings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskStatusHistory_Severity",
                table: "OperationShiftTaskStatusHistory",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTasks_Status",
                table: "OperationShiftTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTasks_Type",
                table: "OperationShiftTasks",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_Status",
                table: "OperationShiftParameterReadings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_Type",
                table: "OperationShiftParameterReadings",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShiftTasks_OperationShifts_OperationShiftId",
                table: "OperationShiftTasks",
                column: "OperationShiftId",
                principalTable: "OperationShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
