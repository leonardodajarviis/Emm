using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActionMarker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTasks_OperationShiftId_Order",
                table: "OperationShiftTasks");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTasks_Order",
                table: "OperationShiftTasks");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "OperationShiftTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "OperationShiftTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTasks_OperationShiftId_Order",
                table: "OperationShiftTasks",
                columns: new[] { "OperationShiftId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTasks_Order",
                table: "OperationShiftTasks",
                column: "Order");
        }
    }
}
