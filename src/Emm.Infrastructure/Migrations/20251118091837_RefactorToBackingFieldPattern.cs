using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToBackingFieldPattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationLogs_OperationShifts_OperationShiftId1",
                table: "OperationLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationShiftAssets_OperationShifts_OperationShiftId1",
                table: "OperationShiftAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationTasks_OperationShifts_OperationShiftId1",
                table: "OperationTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterReadings_OperationTasks_OperationTaskId1",
                table: "ParameterReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCheckpoints_OperationTasks_OperationTaskId1",
                table: "TaskCheckpoints");

            migrationBuilder.DropIndex(
                name: "IX_TaskCheckpoints_OperationTaskId1",
                table: "TaskCheckpoints");

            migrationBuilder.DropIndex(
                name: "IX_ParameterReadings_OperationTaskId1",
                table: "ParameterReadings");

            migrationBuilder.DropIndex(
                name: "IX_OperationTasks_OperationShiftId1",
                table: "OperationTasks");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftAssets_OperationShiftId1",
                table: "OperationShiftAssets");

            migrationBuilder.DropIndex(
                name: "IX_OperationLogs_OperationShiftId1",
                table: "OperationLogs");

            migrationBuilder.DropColumn(
                name: "OperationTaskId1",
                table: "TaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "OperationTaskId1",
                table: "ParameterReadings");

            migrationBuilder.DropColumn(
                name: "OperationShiftId1",
                table: "OperationTasks");

            migrationBuilder.DropColumn(
                name: "OperationShiftId1",
                table: "OperationShiftAssets");

            migrationBuilder.DropColumn(
                name: "OperationShiftId1",
                table: "OperationLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OperationTaskId1",
                table: "TaskCheckpoints",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OperationTaskId1",
                table: "ParameterReadings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OperationShiftId1",
                table: "OperationTasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OperationShiftId1",
                table: "OperationShiftAssets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OperationShiftId1",
                table: "OperationLogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckpoints_OperationTaskId1",
                table: "TaskCheckpoints",
                column: "OperationTaskId1");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_OperationTaskId1",
                table: "ParameterReadings",
                column: "OperationTaskId1");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTasks_OperationShiftId1",
                table: "OperationTasks",
                column: "OperationShiftId1");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_OperationShiftId1",
                table: "OperationShiftAssets",
                column: "OperationShiftId1");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_OperationShiftId1",
                table: "OperationLogs",
                column: "OperationShiftId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationLogs_OperationShifts_OperationShiftId1",
                table: "OperationLogs",
                column: "OperationShiftId1",
                principalTable: "OperationShifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShiftAssets_OperationShifts_OperationShiftId1",
                table: "OperationShiftAssets",
                column: "OperationShiftId1",
                principalTable: "OperationShifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationTasks_OperationShifts_OperationShiftId1",
                table: "OperationTasks",
                column: "OperationShiftId1",
                principalTable: "OperationShifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterReadings_OperationTasks_OperationTaskId1",
                table: "ParameterReadings",
                column: "OperationTaskId1",
                principalTable: "OperationTasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCheckpoints_OperationTasks_OperationTaskId1",
                table: "TaskCheckpoints",
                column: "OperationTaskId1",
                principalTable: "OperationTasks",
                principalColumn: "Id");
        }
    }
}
