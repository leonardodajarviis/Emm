using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LN70COL1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLooked",
                table: "ShiftLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "ShiftLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "ShiftLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupNumber",
                table: "ShiftLogParameterReadings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OperationShiftId",
                table: "ShiftLogParameterReadings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ParameterType",
                table: "ShiftLogParameterReadings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftLogId1",
                table: "ShiftLogParameterReadings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CheckpointOrder",
                table: "ShiftLogCheckpoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentShiftLogId",
                table: "OperationShifts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OperationShiftReadingSnapshots",
                columns: table => new
                {
                    OperationShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftReadingSnapshots", x => new { x.OperationShiftId, x.AssetId, x.ParameterId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_LocationId",
                table: "ShiftLogs",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_OperationShiftId",
                table: "ShiftLogParameterReadings",
                column: "OperationShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_ShiftLogId1",
                table: "ShiftLogParameterReadings",
                column: "ShiftLogId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogParameterReadings_OperationShifts_OperationShiftId",
                table: "ShiftLogParameterReadings",
                column: "OperationShiftId",
                principalTable: "OperationShifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogParameterReadings_ShiftLogs_ShiftLogId1",
                table: "ShiftLogParameterReadings",
                column: "ShiftLogId1",
                principalTable: "ShiftLogs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogs_Locations_LocationId",
                table: "ShiftLogs",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogParameterReadings_OperationShifts_OperationShiftId",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogParameterReadings_ShiftLogs_ShiftLogId1",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogs_Locations_LocationId",
                table: "ShiftLogs");

            migrationBuilder.DropTable(
                name: "OperationShiftReadingSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogs_LocationId",
                table: "ShiftLogs");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogParameterReadings_OperationShiftId",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogParameterReadings_ShiftLogId1",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropColumn(
                name: "IsLooked",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "GroupNumber",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropColumn(
                name: "OperationShiftId",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropColumn(
                name: "ParameterType",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropColumn(
                name: "ShiftLogId1",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropColumn(
                name: "CheckpointOrder",
                table: "ShiftLogCheckpoints");

            migrationBuilder.DropColumn(
                name: "CurrentShiftLogId",
                table: "OperationShifts");
        }
    }
}
