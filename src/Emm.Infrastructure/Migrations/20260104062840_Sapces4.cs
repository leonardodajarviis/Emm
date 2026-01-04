using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sapces4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCodeGenerated",
                table: "Assets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCodeGenerated",
                table: "Assets");
        }
    }
}
