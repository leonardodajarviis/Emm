using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToBoxName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetBoxes_OperationShiftId_BoxName_Unique",
                table: "OperationShiftAssetBoxes",
                columns: new[] { "OperationShiftId", "BoxName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationShiftAssetBoxes_OperationShiftId_BoxName_Unique",
                table: "OperationShiftAssetBoxes");
        }
    }
}
