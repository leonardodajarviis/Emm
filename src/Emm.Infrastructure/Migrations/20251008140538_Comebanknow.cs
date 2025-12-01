using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Comebanknow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanDefinitions_ParameterCatalogs_ParameterId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanInstances_ParameterCatalogs_ParameterId",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanInstances_AssetId",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanInstances_ParameterId",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanDefinitions_AssetModelId_IsActive",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanDefinitions_ParameterId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "Max",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "ParameterId",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "Max",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "ParameterId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "Value",
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

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "MaintenancePlanJobStepInstances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MaintenancePlanJobStepInstances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "MaintenancePlanInstances",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MaintenancePlanInstances",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "MaintenancePlanInstances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MaintenancePlanInstances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MaintenancePlanDefinitionId",
                table: "MaintenancePlanInstances",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MaintenancePlanInstances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "MaintenancePlanInstances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDate",
                table: "MaintenancePlanInstances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MaintenancePlanInstances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TriggerSource",
                table: "MaintenancePlanInstances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MaintenancePlanDefinitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MaintenancePlanDefinitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanType",
                table: "MaintenancePlanDefinitions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ParameterBasedMaintenanceTriggerInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanInstanceId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterId = table.Column<long>(type: "bigint", nullable: false),
                    TriggerValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ActualValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TriggerDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                name: "ParameterBasedMaintenanceTriggers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterId = table.Column<long>(type: "bigint", nullable: false),
                    TriggerValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TriggerCondition = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaintenancePlanDefinitionId1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterBasedMaintenanceTriggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitions_MaintenancePlanDefinitionId",
                        column: x => x.MaintenancePlanDefinitionId,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
                        column: x => x.MaintenancePlanDefinitionId1,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimeBasedMaintenanceTriggerInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanInstanceId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualTriggerDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "TimeBasedMaintenanceTriggers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduleType = table.Column<int>(type: "int", nullable: false),
                    IntervalValue = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HourOfDay = table.Column<int>(type: "int", nullable: true),
                    DayOfWeek = table.Column<int>(type: "int", nullable: true),
                    DayOfMonth = table.Column<int>(type: "int", nullable: true),
                    MonthOfYear = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaintenancePlanDefinitionId1 = table.Column<long>(type: "bigint", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_TimeBasedMaintenanceTriggers_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
                        column: x => x.MaintenancePlanDefinitionId1,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions",
                column: "MaintenancePlanDefinitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanInstances_AssetId_PlanDefinitionId",
                table: "MaintenancePlanInstances",
                columns: new[] { "AssetId", "MaintenancePlanDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanInstances_MaintenancePlanDefinitionId",
                table: "MaintenancePlanInstances",
                column: "MaintenancePlanDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_AssetModelId_IsActive_PlanType",
                table: "MaintenancePlanDefinitions",
                columns: new[] { "AssetModelId", "IsActive", "PlanType" });

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
                name: "IX_ParameterBasedMaintenanceTriggers_IsActive",
                table: "ParameterBasedMaintenanceTriggers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitionId",
                table: "ParameterBasedMaintenanceTriggers",
                column: "MaintenancePlanDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitionId_ParameterId_IsActive",
                table: "ParameterBasedMaintenanceTriggers",
                columns: new[] { "MaintenancePlanDefinitionId", "ParameterId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitionId1",
                table: "ParameterBasedMaintenanceTriggers",
                column: "MaintenancePlanDefinitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_ParameterId",
                table: "ParameterBasedMaintenanceTriggers",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_ParameterId_IsActive",
                table: "ParameterBasedMaintenanceTriggers",
                columns: new[] { "ParameterId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_TriggerCondition",
                table: "ParameterBasedMaintenanceTriggers",
                column: "TriggerCondition");

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
                name: "IX_TimeBasedMaintenanceTriggers_MaintenancePlanDefinitionId1",
                table: "TimeBasedMaintenanceTriggers",
                column: "MaintenancePlanDefinitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggers_ScheduleType",
                table: "TimeBasedMaintenanceTriggers",
                column: "ScheduleType");

            migrationBuilder.CreateIndex(
                name: "IX_TimeBasedMaintenanceTriggers_StartDate_EndDate",
                table: "TimeBasedMaintenanceTriggers",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanInstances_MaintenancePlanDefinitions_MaintenancePlanDefinitionId",
                table: "MaintenancePlanInstances",
                column: "MaintenancePlanDefinitionId",
                principalTable: "MaintenancePlanDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions",
                column: "MaintenancePlanDefinitionId1",
                principalTable: "MaintenancePlanDefinitions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanInstances_MaintenancePlanDefinitions_MaintenancePlanDefinitionId",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitions_MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions");

            migrationBuilder.DropTable(
                name: "ParameterBasedMaintenanceTriggerInstances");

            migrationBuilder.DropTable(
                name: "ParameterBasedMaintenanceTriggers");

            migrationBuilder.DropTable(
                name: "TimeBasedMaintenanceTriggerInstances");

            migrationBuilder.DropTable(
                name: "TimeBasedMaintenanceTriggers");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanInstances_AssetId_PlanDefinitionId",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanInstances_MaintenancePlanDefinitionId",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanDefinitions_AssetModelId_IsActive_PlanType",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "MaintenancePlanJobStepInstances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MaintenancePlanJobStepInstances");

            migrationBuilder.DropColumn(
                name: "MaintenancePlanDefinitionId1",
                table: "MaintenancePlanJobStepDefinitions");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "MaintenancePlanDefinitionId",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "ScheduledDate",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "TriggerSource",
                table: "MaintenancePlanInstances");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "PlanType",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "OutboxMessages",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "MaintenancePlanInstances",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MaintenancePlanInstances",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<decimal>(
                name: "Max",
                table: "MaintenancePlanInstances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Min",
                table: "MaintenancePlanInstances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "ParameterId",
                table: "MaintenancePlanInstances",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "MaintenancePlanInstances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Max",
                table: "MaintenancePlanDefinitions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Min",
                table: "MaintenancePlanDefinitions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "ParameterId",
                table: "MaintenancePlanDefinitions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "MaintenancePlanDefinitions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanInstances_AssetId",
                table: "MaintenancePlanInstances",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanInstances_ParameterId",
                table: "MaintenancePlanInstances",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_AssetModelId_IsActive",
                table: "MaintenancePlanDefinitions",
                columns: new[] { "AssetModelId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_ParameterId",
                table: "MaintenancePlanDefinitions",
                column: "ParameterId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanDefinitions_ParameterCatalogs_ParameterId",
                table: "MaintenancePlanDefinitions",
                column: "ParameterId",
                principalTable: "ParameterCatalogs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanInstances_ParameterCatalogs_ParameterId",
                table: "MaintenancePlanInstances",
                column: "ParameterId",
                principalTable: "ParameterCatalogs",
                principalColumn: "Id");
        }
    }
}
