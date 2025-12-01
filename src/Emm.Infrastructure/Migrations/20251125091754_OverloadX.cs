using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OverloadX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AssetParameters");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AssetParameters");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "AssetParameters",
                newName: "ValueToMaintenance");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentValue",
                table: "AssetParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ParameterCode",
                table: "AssetParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParameterName",
                table: "AssetParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "AssetParameters",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentValue",
                table: "AssetParameters");

            migrationBuilder.DropColumn(
                name: "ParameterCode",
                table: "AssetParameters");

            migrationBuilder.DropColumn(
                name: "ParameterName",
                table: "AssetParameters");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "AssetParameters");

            migrationBuilder.RenameColumn(
                name: "ValueToMaintenance",
                table: "AssetParameters",
                newName: "Value");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AssetParameters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AssetParameters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
