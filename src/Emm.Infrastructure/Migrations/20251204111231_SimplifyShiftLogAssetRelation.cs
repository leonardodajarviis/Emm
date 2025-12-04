using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyShiftLogAssetRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftLogAssets");

            migrationBuilder.AddColumn<long>(
                name: "AssetId",
                table: "ShiftLogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GroupId",
                table: "ShiftLogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_AssetId",
                table: "ShiftLogs",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_GroupId",
                table: "ShiftLogs",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShiftLogs_AssetId",
                table: "ShiftLogs");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogs_GroupId",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ShiftLogs");

            migrationBuilder.CreateTable(
                name: "ShiftLogAssets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ShiftLogId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftLogAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftLogAssets_ShiftLogs_ShiftLogId",
                        column: x => x.ShiftLogId,
                        principalTable: "ShiftLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogAssets_AssetId",
                table: "ShiftLogAssets",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogAssets_ShiftLogId",
                table: "ShiftLogAssets",
                column: "ShiftLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogAssets_ShiftLogId_AssetId",
                table: "ShiftLogAssets",
                columns: new[] { "ShiftLogId", "AssetId" },
                unique: true);
        }
    }
}
