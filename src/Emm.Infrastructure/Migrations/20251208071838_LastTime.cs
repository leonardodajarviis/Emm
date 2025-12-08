using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LastTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ParameterCatalogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ParameterCatalogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "ParameterCatalogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCodeGenerated",
                table: "ParameterCatalogs",
                type: "bit",
                maxLength: 50,
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "ParameterCatalogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "AssetCategories",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCodeGenerated",
                table: "AssetCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "AssetCategories",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterCatalogs_CreatedByUserId",
                table: "ParameterCatalogs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterCatalogs_UpdatedByUserId",
                table: "ParameterCatalogs",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_CreatedByUserId",
                table: "AssetCategories",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_UpdatedByUserId",
                table: "AssetCategories",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategories_Users_CreatedByUserId",
                table: "AssetCategories",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategories_Users_UpdatedByUserId",
                table: "AssetCategories",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterCatalogs_Users_CreatedByUserId",
                table: "ParameterCatalogs",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterCatalogs_Users_UpdatedByUserId",
                table: "ParameterCatalogs",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategories_Users_CreatedByUserId",
                table: "AssetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategories_Users_UpdatedByUserId",
                table: "AssetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterCatalogs_Users_CreatedByUserId",
                table: "ParameterCatalogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterCatalogs_Users_UpdatedByUserId",
                table: "ParameterCatalogs");

            migrationBuilder.DropIndex(
                name: "IX_ParameterCatalogs_CreatedByUserId",
                table: "ParameterCatalogs");

            migrationBuilder.DropIndex(
                name: "IX_ParameterCatalogs_UpdatedByUserId",
                table: "ParameterCatalogs");

            migrationBuilder.DropIndex(
                name: "IX_AssetCategories_CreatedByUserId",
                table: "AssetCategories");

            migrationBuilder.DropIndex(
                name: "IX_AssetCategories_UpdatedByUserId",
                table: "AssetCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ParameterCatalogs");

            migrationBuilder.DropColumn(
                name: "IsCodeGenerated",
                table: "ParameterCatalogs");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ParameterCatalogs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AssetCategories");

            migrationBuilder.DropColumn(
                name: "IsCodeGenerated",
                table: "AssetCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AssetCategories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ParameterCatalogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ParameterCatalogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
