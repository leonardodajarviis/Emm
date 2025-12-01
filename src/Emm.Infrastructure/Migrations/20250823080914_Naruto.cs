using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Naruto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationShiftAssets_AssignedOperatorId",
                table: "OperationShiftAssets");

            migrationBuilder.DropColumn(
                name: "AssignedOperatorId",
                table: "OperationShiftAssets");

            migrationBuilder.DropColumn(
                name: "AssignedOperatorName",
                table: "OperationShiftAssets");

            migrationBuilder.AddColumn<long>(
                name: "OrganizationUnitId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PrimaryEmpolyeeId",
                table: "OperationShifts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizationUnitId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrimaryEmpolyeeId",
                table: "OperationShifts");

            migrationBuilder.AddColumn<long>(
                name: "AssignedOperatorId",
                table: "OperationShiftAssets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedOperatorName",
                table: "OperationShiftAssets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_AssignedOperatorId",
                table: "OperationShiftAssets",
                column: "AssignedOperatorId");
        }
    }
}
