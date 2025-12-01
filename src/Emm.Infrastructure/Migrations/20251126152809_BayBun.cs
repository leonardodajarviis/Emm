using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BayBun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "ShiftLogs",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "ShiftLogs",
                newName: "EndTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "ShiftLogs",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "ShiftLogs",
                newName: "CompletedAt");
        }
    }
}
