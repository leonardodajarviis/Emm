using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPrimaryEmployeeIdTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "AssetModelParameters");

            migrationBuilder.RenameColumn(
                name: "PrimaryEmpolyeeId",
                table: "OperationShifts",
                newName: "PrimaryEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_OrganizationUnitId",
                table: "OperationShifts",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_PrimaryEmployeeId",
                table: "OperationShifts",
                column: "PrimaryEmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationShifts_OrganizationUnitId",
                table: "OperationShifts");

            migrationBuilder.DropIndex(
                name: "IX_OperationShifts_PrimaryEmployeeId",
                table: "OperationShifts");

            migrationBuilder.RenameColumn(
                name: "PrimaryEmployeeId",
                table: "OperationShifts",
                newName: "PrimaryEmpolyeeId");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "AssetModelParameters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
