using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NORLIVE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropColumn(
                name: "ParameterUnit",
                table: "AssetParameters");

            migrationBuilder.DropColumn(
                name: "ValueToMaintenance",
                table: "AssetParameters");

            migrationBuilder.AddColumn<bool>(
                name: "IsLooked",
                table: "UnitOfMeasures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitOfMeasureId",
                table: "ShiftLogParameterReadings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "ParameterName",
                table: "AssetParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParameterCode",
                table: "AssetParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitOfMeasureId",
                table: "AssetParameters",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_UnitOfMeasureId",
                table: "ShiftLogParameterReadings",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParameters_UnitOfMeasureId",
                table: "AssetParameters",
                column: "UnitOfMeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetParameters_UnitOfMeasures_UnitOfMeasureId",
                table: "AssetParameters",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogParameterReadings_UnitOfMeasures_UnitOfMeasureId",
                table: "ShiftLogParameterReadings",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetParameters_UnitOfMeasures_UnitOfMeasureId",
                table: "AssetParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogParameterReadings_UnitOfMeasures_UnitOfMeasureId",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogParameterReadings_UnitOfMeasureId",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropIndex(
                name: "IX_AssetParameters_UnitOfMeasureId",
                table: "AssetParameters");

            migrationBuilder.DropColumn(
                name: "IsLooked",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                table: "AssetParameters");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "ShiftLogParameterReadings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ParameterName",
                table: "AssetParameters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ParameterCode",
                table: "AssetParameters",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ParameterUnit",
                table: "AssetParameters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueToMaintenance",
                table: "AssetParameters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
