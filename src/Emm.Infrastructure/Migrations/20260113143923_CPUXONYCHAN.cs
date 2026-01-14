using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CPUXONYCHAN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssetCode",
                table: "ShiftLogItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasureCode",
                table: "ShiftLogItems",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitOfMeasureCategoryId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UnitOfMeasureCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BaseUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Audit_CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Audit_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Audit_ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasureCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasureCategoryLines",
                columns: table => new
                {
                    UnitOfMeasureCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasureCategoryLines", x => new { x.UnitOfMeasureCategoryId, x.UnitOfMeasureId });
                    table.ForeignKey(
                        name: "FK_UnitOfMeasureCategoryLines_UnitOfMeasureCategories_UnitOfMeasureCategoryId",
                        column: x => x.UnitOfMeasureCategoryId,
                        principalTable: "UnitOfMeasureCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitOfMeasureCategoryLines_UnitOfMeasures_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogItems_AssetId",
                table: "ShiftLogItems",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogItems_UnitOfMeasureId",
                table: "ShiftLogItems",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UnitOfMeasureCategoryId",
                table: "Items",
                column: "UnitOfMeasureCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasureCategories_Name",
                table: "UnitOfMeasureCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasureCategoryLines_UnitOfMeasureId",
                table: "UnitOfMeasureCategoryLines",
                column: "UnitOfMeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_UnitOfMeasureCategories_UnitOfMeasureCategoryId",
                table: "Items",
                column: "UnitOfMeasureCategoryId",
                principalTable: "UnitOfMeasureCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogItems_Assets_AssetId",
                table: "ShiftLogItems",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogItems_Items_ItemId",
                table: "ShiftLogItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogItems_UnitOfMeasures_UnitOfMeasureId",
                table: "ShiftLogItems",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_UnitOfMeasureCategories_UnitOfMeasureCategoryId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogItems_Assets_AssetId",
                table: "ShiftLogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogItems_Items_ItemId",
                table: "ShiftLogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogItems_UnitOfMeasures_UnitOfMeasureId",
                table: "ShiftLogItems");

            migrationBuilder.DropTable(
                name: "UnitOfMeasureCategoryLines");

            migrationBuilder.DropTable(
                name: "UnitOfMeasureCategories");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogItems_AssetId",
                table: "ShiftLogItems");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogItems_UnitOfMeasureId",
                table: "ShiftLogItems");

            migrationBuilder.DropIndex(
                name: "IX_Items_UnitOfMeasureCategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureCode",
                table: "ShiftLogItems");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureCategoryId",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "AssetCode",
                table: "ShiftLogItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
