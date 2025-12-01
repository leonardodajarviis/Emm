using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PreX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrimaryEmployeeId",
                table: "OperationShifts",
                newName: "PrimaryUserId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShifts_PrimaryEmployeeId",
                table: "OperationShifts",
                newName: "IX_OperationShifts_PrimaryUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrimaryUserId",
                table: "OperationShifts",
                newName: "PrimaryEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_OperationShifts_PrimaryUserId",
                table: "OperationShifts",
                newName: "IX_OperationShifts_PrimaryEmployeeId");
        }
    }
}
