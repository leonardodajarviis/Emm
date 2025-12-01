using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AnhTanDepTrai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UnitOfMeasureId",
                table: "ParameterCatalogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterCatalogs_UnitOfMeasureId",
                table: "ParameterCatalogs",
                column: "UnitOfMeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterCatalogs_UnitOfMeasures_UnitOfMeasureId",
                table: "ParameterCatalogs",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterCatalogs_UnitOfMeasures_UnitOfMeasureId",
                table: "ParameterCatalogs");

            migrationBuilder.DropIndex(
                name: "IX_ParameterCatalogs_UnitOfMeasureId",
                table: "ParameterCatalogs");

            migrationBuilder.AlterColumn<long>(
                name: "UnitOfMeasureId",
                table: "ParameterCatalogs",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
