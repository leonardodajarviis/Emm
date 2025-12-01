using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HePhim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
            //     table: "ParameterBasedMaintenanceTriggers");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_TimeBasedMaintenanceTriggers_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
            //     table: "TimeBasedMaintenanceTriggers");

            // migrationBuilder.DropIndex(
            //     name: "IX_TimeBasedMaintenanceTriggers_MaintenancePlanDefinitionId1",
            //     table: "TimeBasedMaintenanceTriggers");

            // migrationBuilder.DropIndex(
            //     name: "IX_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitionId1",
            //     table: "ParameterBasedMaintenanceTriggers");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions");

            // migrationBuilder.DropColumn(
            //     name: "MaintenancePlanDefinitionId1",
            //     table: "TimeBasedMaintenanceTriggers");

            // migrationBuilder.DropColumn(
            //     name: "MaintenancePlanDefinitionId1",
            //     table: "ParameterBasedMaintenanceTriggers");

            migrationBuilder.DropColumn(
                name: "MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MaintenancePlanDefinitionId1",
                table: "TimeBasedMaintenanceTriggers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MaintenancePlanDefinitionId1",
                table: "ParameterBasedMaintenanceTriggers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggers_MaintenancePlanDefinitionId1",
                table: "TimeBasedMaintenanceTriggers",
                column: "MaintenancePlanDefinitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitionId1",
                table: "ParameterBasedMaintenanceTriggers",
                column: "MaintenancePlanDefinitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions",
                column: "MaintenancePlanDefinitionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions",
                column: "MaintenancePlanDefinitionId1",
                principalTable: "MaintenancePlanDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
                table: "ParameterBasedMaintenanceTriggers",
                column: "MaintenancePlanDefinitionId1",
                principalTable: "MaintenancePlanDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeBasedMaintenanceTriggers_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
                table: "TimeBasedMaintenanceTriggers",
                column: "MaintenancePlanDefinitionId1",
                principalTable: "MaintenancePlanDefinitions",
                principalColumn: "Id");
        }
    }
}
