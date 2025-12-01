using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameOperationShiftTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationLogs_OperationShifts_OperationShiftId",
                table: "OperationLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatusHistory_OperationTasks_OperationTaskId",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationTasks_OperationShifts_OperationShiftId",
                table: "OperationTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterReadings_OperationTasks_OperationTaskId",
                table: "ParameterReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCheckpoints_OperationTasks_OperationTaskId",
                table: "TaskCheckpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCheckpoints",
                table: "TaskCheckpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParameterReadings",
                table: "ParameterReadings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationTasks",
                table: "OperationTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationLogs",
                table: "OperationLogs");

            migrationBuilder.RenameTable(
                name: "TaskCheckpoints",
                newName: "OperationShiftTaskCheckpoints");

            migrationBuilder.RenameTable(
                name: "ParameterReadings",
                newName: "OperationShiftParameterReadings");

            migrationBuilder.RenameTable(
                name: "OperationTasks",
                newName: "OperationShiftTasks");

            migrationBuilder.RenameTable(
                name: "OperationLogs",
                newName: "OperationShiftLogs");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheckpoints_Status",
                table: "OperationShiftTaskCheckpoints",
                newName: "IX_OperationShiftTaskCheckpoints_Status");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheckpoints_OperationTaskId",
                table: "OperationShiftTaskCheckpoints",
                newName: "IX_OperationShiftTaskCheckpoints_OperationTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheckpoints_IsRequired",
                table: "OperationShiftTaskCheckpoints",
                newName: "IX_OperationShiftTaskCheckpoints_IsRequired");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheckpoints_CreatedAt",
                table: "OperationShiftTaskCheckpoints",
                newName: "IX_OperationShiftTaskCheckpoints_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheckpoints_CompletedAt",
                table: "OperationShiftTaskCheckpoints",
                newName: "IX_OperationShiftTaskCheckpoints_CompletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_Type",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_Type");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_Status",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_Status");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_ReadingTime",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_ReadingTime");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_ParameterId",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_ParameterId");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_ParameterCode",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_ParameterCode");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_OperationTaskId_AssetId",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_OperationTaskId_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_OperationTaskId",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_OperationTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_CreatedAt",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_AssetId_ParameterId_ReadingTime",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_AssetId_ParameterId_ReadingTime");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterReadings_AssetId",
                table: "OperationShiftParameterReadings",
                newName: "IX_OperationShiftParameterReadings_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationTasks_Type",
                table: "OperationShiftTasks",
                newName: "IX_OperationShiftTasks_Type");

            migrationBuilder.RenameIndex(
                name: "IX_OperationTasks_Status",
                table: "OperationShiftTasks",
                newName: "IX_OperationShiftTasks_Status");

            migrationBuilder.RenameIndex(
                name: "IX_OperationTasks_Order",
                table: "OperationShiftTasks",
                newName: "IX_OperationShiftTasks_Order");

            migrationBuilder.RenameIndex(
                name: "IX_OperationTasks_OperationShiftId_Order",
                table: "OperationShiftTasks",
                newName: "IX_OperationShiftTasks_OperationShiftId_Order");

            migrationBuilder.RenameIndex(
                name: "IX_OperationTasks_OperationShiftId",
                table: "OperationShiftTasks",
                newName: "IX_OperationShiftTasks_OperationShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationTasks_CreatedAt",
                table: "OperationShiftTasks",
                newName: "IX_OperationShiftTasks_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OperationLogs_Type_Level",
                table: "OperationShiftLogs",
                newName: "IX_OperationShiftLogs_Type_Level");

            migrationBuilder.RenameIndex(
                name: "IX_OperationLogs_Type",
                table: "OperationShiftLogs",
                newName: "IX_OperationShiftLogs_Type");

            migrationBuilder.RenameIndex(
                name: "IX_OperationLogs_Timestamp",
                table: "OperationShiftLogs",
                newName: "IX_OperationShiftLogs_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_OperationLogs_OperationShiftId_Timestamp",
                table: "OperationShiftLogs",
                newName: "IX_OperationShiftLogs_OperationShiftId_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_OperationLogs_OperationShiftId",
                table: "OperationShiftLogs",
                newName: "IX_OperationShiftLogs_OperationShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationLogs_Level",
                table: "OperationShiftLogs",
                newName: "IX_OperationShiftLogs_Level");

            migrationBuilder.RenameIndex(
                name: "IX_OperationLogs_CreatedAt",
                table: "OperationShiftLogs",
                newName: "IX_OperationShiftLogs_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationShiftTaskCheckpoints",
                table: "OperationShiftTaskCheckpoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationShiftParameterReadings",
                table: "OperationShiftParameterReadings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationShiftTasks",
                table: "OperationShiftTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationShiftLogs",
                table: "OperationShiftLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShiftLogs_OperationShifts_OperationShiftId",
                table: "OperationShiftLogs",
                column: "OperationShiftId",
                principalTable: "OperationShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShiftParameterReadings_OperationShiftTasks_OperationTaskId",
                table: "OperationShiftParameterReadings",
                column: "OperationTaskId",
                principalTable: "OperationShiftTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShiftTaskCheckpoints_OperationShiftTasks_OperationTaskId",
                table: "OperationShiftTaskCheckpoints",
                column: "OperationTaskId",
                principalTable: "OperationShiftTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShiftTasks_OperationShifts_OperationShiftId",
                table: "OperationShiftTasks",
                column: "OperationShiftId",
                principalTable: "OperationShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShiftTaskStatusHistory_OperationShiftTasks_OperationTaskId",
                table: "OperationShiftTaskStatusHistory",
                column: "OperationTaskId",
                principalTable: "OperationShiftTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationShiftLogs_OperationShifts_OperationShiftId",
                table: "OperationShiftLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationShiftParameterReadings_OperationShiftTasks_OperationTaskId",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationShiftTaskCheckpoints_OperationShiftTasks_OperationTaskId",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationShiftTasks_OperationShifts_OperationShiftId",
                table: "OperationShiftTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationShiftTaskStatusHistory_OperationShiftTasks_OperationTaskId",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationShiftTasks",
                table: "OperationShiftTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationShiftTaskCheckpoints",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationShiftParameterReadings",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OperationShiftLogs",
                table: "OperationShiftLogs");

            migrationBuilder.RenameTable(
                name: "OperationShiftTasks",
                newName: "OperationTasks");

            migrationBuilder.RenameTable(
                name: "OperationShiftTaskCheckpoints",
                newName: "TaskCheckpoints");

            migrationBuilder.RenameTable(
                name: "OperationShiftParameterReadings",
                newName: "ParameterReadings");

            migrationBuilder.RenameTable(
                name: "OperationShiftLogs",
                newName: "OperationLogs");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTasks_Type",
                table: "OperationTasks",
                newName: "IX_OperationTasks_Type");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTasks_Status",
                table: "OperationTasks",
                newName: "IX_OperationTasks_Status");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTasks_Order",
                table: "OperationTasks",
                newName: "IX_OperationTasks_Order");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTasks_OperationShiftId_Order",
                table: "OperationTasks",
                newName: "IX_OperationTasks_OperationShiftId_Order");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTasks_OperationShiftId",
                table: "OperationTasks",
                newName: "IX_OperationTasks_OperationShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTasks_CreatedAt",
                table: "OperationTasks",
                newName: "IX_OperationTasks_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTaskCheckpoints_Status",
                table: "TaskCheckpoints",
                newName: "IX_TaskCheckpoints_Status");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTaskCheckpoints_OperationTaskId",
                table: "TaskCheckpoints",
                newName: "IX_TaskCheckpoints_OperationTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTaskCheckpoints_IsRequired",
                table: "TaskCheckpoints",
                newName: "IX_TaskCheckpoints_IsRequired");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTaskCheckpoints_CreatedAt",
                table: "TaskCheckpoints",
                newName: "IX_TaskCheckpoints_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftTaskCheckpoints_CompletedAt",
                table: "TaskCheckpoints",
                newName: "IX_TaskCheckpoints_CompletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_Type",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_Type");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_Status",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_Status");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_ReadingTime",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_ReadingTime");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_ParameterId",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_ParameterId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_ParameterCode",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_ParameterCode");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_OperationTaskId_AssetId",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_OperationTaskId_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_OperationTaskId",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_OperationTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_CreatedAt",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_AssetId_ParameterId_ReadingTime",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_AssetId_ParameterId_ReadingTime");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftParameterReadings_AssetId",
                table: "ParameterReadings",
                newName: "IX_ParameterReadings_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftLogs_Type_Level",
                table: "OperationLogs",
                newName: "IX_OperationLogs_Type_Level");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftLogs_Type",
                table: "OperationLogs",
                newName: "IX_OperationLogs_Type");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftLogs_Timestamp",
                table: "OperationLogs",
                newName: "IX_OperationLogs_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftLogs_OperationShiftId_Timestamp",
                table: "OperationLogs",
                newName: "IX_OperationLogs_OperationShiftId_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftLogs_OperationShiftId",
                table: "OperationLogs",
                newName: "IX_OperationLogs_OperationShiftId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftLogs_Level",
                table: "OperationLogs",
                newName: "IX_OperationLogs_Level");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShiftLogs_CreatedAt",
                table: "OperationLogs",
                newName: "IX_OperationLogs_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationTasks",
                table: "OperationTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCheckpoints",
                table: "TaskCheckpoints",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParameterReadings",
                table: "ParameterReadings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OperationLogs",
                table: "OperationLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationLogs_OperationShifts_OperationShiftId",
                table: "OperationLogs",
                column: "OperationShiftId",
                principalTable: "OperationShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatusHistory_OperationTasks_OperationTaskId",
                table: "OperationShiftTaskStatusHistory",
                column: "OperationTaskId",
                principalTable: "OperationTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationTasks_OperationShifts_OperationShiftId",
                table: "OperationTasks",
                column: "OperationShiftId",
                principalTable: "OperationShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterReadings_OperationTasks_OperationTaskId",
                table: "ParameterReadings",
                column: "OperationTaskId",
                principalTable: "OperationTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCheckpoints_OperationTasks_OperationTaskId",
                table: "TaskCheckpoints",
                column: "OperationTaskId",
                principalTable: "OperationTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
