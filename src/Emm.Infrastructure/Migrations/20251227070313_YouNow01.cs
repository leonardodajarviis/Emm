using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class YouNow01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemGroupId",
                table: "MaintenancePlanRequiredItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UnitOfMeasureId",
                table: "MaintenancePlanRequiredItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanRequiredItems_ItemGroupId",
                table: "MaintenancePlanRequiredItems",
                column: "ItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanRequiredItems_UnitOfMeasureId",
                table: "MaintenancePlanRequiredItems",
                column: "UnitOfMeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanRequiredItems_ItemGroups_ItemGroupId",
                table: "MaintenancePlanRequiredItems",
                column: "ItemGroupId",
                principalTable: "ItemGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanRequiredItems_Items_ItemId",
                table: "MaintenancePlanRequiredItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanRequiredItems_UnitOfMeasures_UnitOfMeasureId",
                table: "MaintenancePlanRequiredItems",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanRequiredItems_ItemGroups_ItemGroupId",
                table: "MaintenancePlanRequiredItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanRequiredItems_Items_ItemId",
                table: "MaintenancePlanRequiredItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanRequiredItems_UnitOfMeasures_UnitOfMeasureId",
                table: "MaintenancePlanRequiredItems");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanRequiredItems_ItemGroupId",
                table: "MaintenancePlanRequiredItems");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanRequiredItems_UnitOfMeasureId",
                table: "MaintenancePlanRequiredItems");

            migrationBuilder.DropColumn(
                name: "ItemGroupId",
                table: "MaintenancePlanRequiredItems");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                table: "MaintenancePlanRequiredItems");
        }
    }
}
