using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LN312OL1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Batch",
                table: "ShiftLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EventOrder",
                table: "ShiftLogEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_Batch",
                table: "ShiftLogs",
                column: "Batch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShiftLogs_Batch",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "Batch",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "EventOrder",
                table: "ShiftLogEvents");
        }
    }
}
