using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EmCuaNgayHomQua : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsCodeGenerated",
                table: "ParameterCatalogs",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "0");

            migrationBuilder.AddColumn<bool>(
                name: "IsCodeGenerated",
                table: "AssetModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_CreatedByUserId",
                table: "AssetModels",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_UpdatedByUserId",
                table: "AssetModels",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetModels_Users_CreatedByUserId",
                table: "AssetModels",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetModels_Users_UpdatedByUserId",
                table: "AssetModels",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_Users_CreatedByUserId",
                table: "AssetModels");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_Users_UpdatedByUserId",
                table: "AssetModels");

            migrationBuilder.DropIndex(
                name: "IX_AssetModels_CreatedByUserId",
                table: "AssetModels");

            migrationBuilder.DropIndex(
                name: "IX_AssetModels_UpdatedByUserId",
                table: "AssetModels");

            migrationBuilder.DropColumn(
                name: "IsCodeGenerated",
                table: "AssetModels");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCodeGenerated",
                table: "ParameterCatalogs",
                type: "bit",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
