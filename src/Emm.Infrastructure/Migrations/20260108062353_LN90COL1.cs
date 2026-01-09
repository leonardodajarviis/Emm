using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LN90COL1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TriggerValue",
                table: "ParameterBasedMaintenanceTriggers",
                newName: "ThresholdValue");

            migrationBuilder.RenameColumn(
                name: "MinValue",
                table: "ParameterBasedMaintenanceTriggers",
                newName: "PlusTolerance");

            migrationBuilder.RenameColumn(
                name: "MaxValue",
                table: "ParameterBasedMaintenanceTriggers",
                newName: "MinusTolerance");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OperationShifts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Assets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThresholdValue",
                table: "ParameterBasedMaintenanceTriggers",
                newName: "TriggerValue");

            migrationBuilder.RenameColumn(
                name: "PlusTolerance",
                table: "ParameterBasedMaintenanceTriggers",
                newName: "MinValue");

            migrationBuilder.RenameColumn(
                name: "MinusTolerance",
                table: "ParameterBasedMaintenanceTriggers",
                newName: "MaxValue");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "OperationShifts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
