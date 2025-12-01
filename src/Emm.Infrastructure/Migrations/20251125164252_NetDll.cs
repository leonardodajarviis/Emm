using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NetDll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationShiftLogs");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTaskStatusHistory_IsResolved",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTaskStatusHistory_OperationTaskId_IsResolved",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "AdditionalData",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "RecordedBy",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "ResolvedBy",
                table: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "OperationShiftTaskStatusHistory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalData",
                table: "OperationShiftTaskStatusHistory",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OperationShiftTaskStatusHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "OperationShiftTaskStatusHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecordedBy",
                table: "OperationShiftTaskStatusHistory",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "OperationShiftTaskStatusHistory",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "OperationShiftTaskStatusHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolvedBy",
                table: "OperationShiftTaskStatusHistory",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "OperationShiftTaskStatusHistory",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OperationShiftLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdditionalData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    OperationShiftId = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationShiftLogs_OperationShifts_OperationShiftId",
                        column: x => x.OperationShiftId,
                        principalTable: "OperationShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskStatusHistory_IsResolved",
                table: "OperationShiftTaskStatusHistory",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskStatusHistory_OperationTaskId_IsResolved",
                table: "OperationShiftTaskStatusHistory",
                columns: new[] { "OperationTaskId", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftLogs_CreatedAt",
                table: "OperationShiftLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftLogs_Level",
                table: "OperationShiftLogs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftLogs_OperationShiftId",
                table: "OperationShiftLogs",
                column: "OperationShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftLogs_OperationShiftId_Timestamp",
                table: "OperationShiftLogs",
                columns: new[] { "OperationShiftId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftLogs_Timestamp",
                table: "OperationShiftLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftLogs_Type",
                table: "OperationShiftLogs",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftLogs_Type_Level",
                table: "OperationShiftLogs",
                columns: new[] { "Type", "Level" });
        }
    }
}
