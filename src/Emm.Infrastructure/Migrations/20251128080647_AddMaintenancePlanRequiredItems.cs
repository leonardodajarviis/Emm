using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenancePlanRequiredItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssetCategoryCode",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AssetCategoryId",
                table: "Assets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "AssetCategoryName",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetModelCode",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetModelName",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetTypeCode",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetTypeName",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MaintenancePlanRequiredItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlanRequiredItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanRequiredItems_MaintenancePlanDefinitions_MaintenancePlanDefinitionId",
                        column: x => x.MaintenancePlanDefinitionId,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanRequiredItems_ItemId",
                table: "MaintenancePlanRequiredItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanRequiredItems_MaintenancePlanDefinitionId",
                table: "MaintenancePlanRequiredItems",
                column: "MaintenancePlanDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenancePlanRequiredItems");

            migrationBuilder.DropColumn(
                name: "AssetCategoryCode",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetCategoryName",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetModelCode",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetModelName",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetTypeCode",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetTypeName",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Assets");
        }
    }
}
