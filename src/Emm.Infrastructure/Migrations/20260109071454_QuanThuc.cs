using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class QuanThuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetParameterMaintenances",
                table: "AssetParameterMaintenances");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ShiftLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "ShiftLogItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseIssueSlipId",
                table: "ShiftLogItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ParameterCatalogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AssetParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetParameterMaintenances",
                table: "AssetParameterMaintenances",
                columns: new[] { "AssetId", "MaintenancePlanId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetParameterMaintenances",
                table: "AssetParameterMaintenances");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "ShiftLogItems");

            migrationBuilder.DropColumn(
                name: "WarehouseIssueSlipId",
                table: "ShiftLogItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ParameterCatalogs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AssetParameters");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ShiftLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetParameterMaintenances",
                table: "AssetParameterMaintenances",
                columns: new[] { "AssetId", "ParameterId" });
        }
    }
}
