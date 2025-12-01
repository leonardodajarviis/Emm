using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Kakashi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationShifts_PrimaryOperatorId",
                table: "OperationShifts");

            migrationBuilder.DropColumn(
                name: "PrimaryOperatorName",
                table: "OperationShifts");

            migrationBuilder.RenameColumn(
                name: "PrimaryOperatorId",
                table: "OperationShifts",
                newName: "OrganizationUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_Name",
                table: "Assets",
                newName: "IX_Assets_DisplayName");

            migrationBuilder.AddColumn<long>(
                name: "AssetAdditionId",
                table: "Assets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "Assets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "OrganizationUnitId",
                table: "Assets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetAdditionId",
                table: "Assets",
                column: "AssetAdditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_LocationId",
                table: "Assets",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_OrganizationUnitId",
                table: "Assets",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeId",
                table: "Users",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetAdditions_AssetAdditionId",
                table: "Assets",
                column: "AssetAdditionId",
                principalTable: "AssetAdditions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Locations_LocationId",
                table: "Assets",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_OrganizationUnits_OrganizationUnitId",
                table: "Assets",
                column: "OrganizationUnitId",
                principalTable: "OrganizationUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetAdditions_AssetAdditionId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Locations_LocationId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_OrganizationUnits_OrganizationUnitId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetAdditionId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_LocationId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_OrganizationUnitId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "AssetAdditionId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "OrganizationUnitId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "OrganizationUnitId",
                table: "OperationShifts",
                newName: "PrimaryOperatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_DisplayName",
                table: "Assets",
                newName: "IX_Assets_Name");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryOperatorName",
                table: "OperationShifts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_PrimaryOperatorId",
                table: "OperationShifts",
                column: "PrimaryOperatorId");
        }
    }
}
