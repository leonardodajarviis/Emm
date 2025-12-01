using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserSSio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetParameters_Assets_AssetId",
                table: "AssetParameters");

            migrationBuilder.DropTable(
                name: "MaintenancePlanJobStepInstances");

            migrationBuilder.DropTable(
                name: "ParameterBasedMaintenanceTriggerInstances");

            migrationBuilder.DropTable(
                name: "TimeBasedMaintenanceTriggerInstances");

            migrationBuilder.DropTable(
                name: "MaintenancePlanInstances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaintenancePlanInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TriggerSource = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlanInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanInstances_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanInstances_MaintenancePlanDefinitions_MaintenancePlanDefinitionId",
                        column: x => x.MaintenancePlanDefinitionId,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePlanJobStepInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenancePlanInstanceId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlanJobStepInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanJobStepInstances_MaintenancePlanInstances_MaintenancePlanInstanceId",
                        column: x => x.MaintenancePlanInstanceId,
                        principalTable: "MaintenancePlanInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanJobStepInstances_OrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "OrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ParameterBasedMaintenanceTriggerInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanInstanceId = table.Column<long>(type: "bigint", nullable: false),
                    ActualValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ParameterId = table.Column<long>(type: "bigint", nullable: false),
                    TriggerDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TriggerValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterBasedMaintenanceTriggerInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParameterBasedMaintenanceTriggerInstances_MaintenancePlanInstances_MaintenancePlanInstanceId",
                        column: x => x.MaintenancePlanInstanceId,
                        principalTable: "MaintenancePlanInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeBasedMaintenanceTriggerInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanInstanceId = table.Column<long>(type: "bigint", nullable: false),
                    ActualTriggerDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeBasedMaintenanceTriggerInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeBasedMaintenanceTriggerInstances_MaintenancePlanInstances_MaintenancePlanInstanceId",
                        column: x => x.MaintenancePlanInstanceId,
                        principalTable: "MaintenancePlanInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanInstances_AssetId_PlanDefinitionId",
                table: "MaintenancePlanInstances",
                columns: new[] { "AssetId", "MaintenancePlanDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanInstances_MaintenancePlanDefinitionId",
                table: "MaintenancePlanInstances",
                column: "MaintenancePlanDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepInstances_MaintenancePlanInstanceId",
                table: "MaintenancePlanJobStepInstances",
                column: "MaintenancePlanInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepInstances_OrganizationUnitId",
                table: "MaintenancePlanJobStepInstances",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggerInstances_MaintenancePlanInstanceId",
                table: "ParameterBasedMaintenanceTriggerInstances",
                column: "MaintenancePlanInstanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggerInstances_ParameterId",
                table: "ParameterBasedMaintenanceTriggerInstances",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggerInstances_ParameterId_ActualValue",
                table: "ParameterBasedMaintenanceTriggerInstances",
                columns: new[] { "ParameterId", "ActualValue" });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggerInstances_ParameterId_TriggerDateTime",
                table: "ParameterBasedMaintenanceTriggerInstances",
                columns: new[] { "ParameterId", "TriggerDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggerInstances_TriggerDateTime",
                table: "ParameterBasedMaintenanceTriggerInstances",
                column: "TriggerDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggerInstances_ActualTriggerDate",
                table: "TimeBasedMaintenanceTriggerInstances",
                column: "ActualTriggerDate");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggerInstances_MaintenancePlanInstanceId",
                table: "TimeBasedMaintenanceTriggerInstances",
                column: "MaintenancePlanInstanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggerInstances_ScheduledDate",
                table: "TimeBasedMaintenanceTriggerInstances",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggerInstances_ScheduledDate_ActualTriggerDate",
                table: "TimeBasedMaintenanceTriggerInstances",
                columns: new[] { "ScheduledDate", "ActualTriggerDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParameters_Assets_AssetId",
                table: "AssetParameters",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
