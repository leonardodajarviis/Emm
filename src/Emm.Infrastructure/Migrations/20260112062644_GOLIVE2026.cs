using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GOLIVE2026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogParameterReadings_ShiftLogs_ShiftLogId1",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogParameterReadings_ShiftLogId1",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropColumn(
                name: "ShiftLogId1",
                table: "ShiftLogParameterReadings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ShiftLogId1",
                table: "ShiftLogParameterReadings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_ShiftLogId1",
                table: "ShiftLogParameterReadings",
                column: "ShiftLogId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogParameterReadings_ShiftLogs_ShiftLogId1",
                table: "ShiftLogParameterReadings",
                column: "ShiftLogId1",
                principalTable: "ShiftLogs",
                principalColumn: "Id");
        }
    }
}
