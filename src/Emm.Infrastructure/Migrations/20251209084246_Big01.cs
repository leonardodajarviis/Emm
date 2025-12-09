using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Big01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitOfMeasures_UnitOfMeasures_BaseUnitId",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UnitOfMeasures");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitOfMeasures_UnitOfMeasures_BaseUnitId",
                table: "UnitOfMeasures",
                column: "BaseUnitId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitOfMeasures_UnitOfMeasures_BaseUnitId",
                table: "UnitOfMeasures");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UnitOfMeasures",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UnitOfMeasures",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_UnitOfMeasures_UnitOfMeasures_BaseUnitId",
                table: "UnitOfMeasures",
                column: "BaseUnitId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
