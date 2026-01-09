using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NOTLF023 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThresholdValue",
                table: "ParameterBasedMaintenanceTriggers",
                newName: "Value");

            migrationBuilder.CreateTable(
                name: "AssetParameterMaintenances",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenancePlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlusTolerance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinusTolerance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetParameterMaintenances", x => new { x.AssetId, x.ParameterId });
                    table.ForeignKey(
                        name: "FK_AssetParameterMaintenances_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetParameterMaintenances_MaintenancePlanDefinitions_MaintenancePlanId",
                        column: x => x.MaintenancePlanId,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetParameterMaintenances_MaintenancePlanId",
                table: "AssetParameterMaintenances",
                column: "MaintenancePlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetParameterMaintenances");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ParameterBasedMaintenanceTriggers",
                newName: "ThresholdValue");
        }
    }
}
