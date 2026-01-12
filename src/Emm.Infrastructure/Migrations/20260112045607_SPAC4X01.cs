using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SPAC4X01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanDefinitions_AssetModels_AssetModelId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_TriggerCondition",
                table: "ParameterBasedMaintenanceTriggers");

            migrationBuilder.DropColumn(
                name: "TriggerCondition",
                table: "ParameterBasedMaintenanceTriggers");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShiftReadingSnapshots_OperationShifts_OperationShiftId",
                table: "OperationShiftReadingSnapshots",
                column: "OperationShiftId",
                principalTable: "OperationShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationShiftReadingSnapshots_OperationShifts_OperationShiftId",
                table: "OperationShiftReadingSnapshots");

            migrationBuilder.AddColumn<int>(
                name: "TriggerCondition",
                table: "ParameterBasedMaintenanceTriggers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_TriggerCondition",
                table: "ParameterBasedMaintenanceTriggers",
                column: "TriggerCondition");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanDefinitions_AssetModels_AssetModelId",
                table: "MaintenancePlanDefinitions",
                column: "AssetModelId",
                principalTable: "AssetModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
