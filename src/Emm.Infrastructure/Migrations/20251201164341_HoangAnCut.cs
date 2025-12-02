using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HoangAnCut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShiftLogItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftLogId = table.Column<long>(type: "bigint", nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: true),
                    AssetCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitOfMeasureId = table.Column<long>(type: "bigint", nullable: true),
                    UnitOfMeasureName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftLogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftLogItems_ShiftLogs_ShiftLogId",
                        column: x => x.ShiftLogId,
                        principalTable: "ShiftLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogItems_ItemId",
                table: "ShiftLogItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogItems_ShiftLogId",
                table: "ShiftLogItems",
                column: "ShiftLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftLogItems");
        }
    }
}
