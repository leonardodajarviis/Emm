using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LastWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncidentReports_Users_CreatedById",
                table: "IncidentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentReports_Users_UpdatedById",
                table: "IncidentReports");

            migrationBuilder.DropIndex(
                name: "IX_IncidentReports_CreatedById",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "IncidentReports");

            migrationBuilder.RenameColumn(
                name: "UpdatedById",
                table: "IncidentReports",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_IncidentReports_UpdatedById",
                table: "IncidentReports",
                newName: "IX_IncidentReports_CreatedByUserId");

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "ShiftLogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "ShiftLogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Roles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "Roles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Policies",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "Policies",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Permissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "Permissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "OperationShifts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "OperationShifts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "MaintenancePlanDefinitions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "MaintenancePlanDefinitions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Locations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "Locations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Items",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "Items",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "IncidentReports",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "AssetTypes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCodeGenerated",
                table: "AssetTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "AssetTypes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Assets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "Assets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "AssetModels",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "AssetModels",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "AssetAdditions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedByUserId",
                table: "AssetAdditions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_UpdatedByUserId",
                table: "IncidentReports",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_CreatedByUserId",
                table: "AssetTypes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_UpdatedByUserId",
                table: "AssetTypes",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTypes_Users_CreatedByUserId",
                table: "AssetTypes",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTypes_Users_UpdatedByUserId",
                table: "AssetTypes",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentReports_Users_CreatedByUserId",
                table: "IncidentReports",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentReports_Users_UpdatedByUserId",
                table: "IncidentReports",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetTypes_Users_CreatedByUserId",
                table: "AssetTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTypes_Users_UpdatedByUserId",
                table: "AssetTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentReports_Users_CreatedByUserId",
                table: "IncidentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentReports_Users_UpdatedByUserId",
                table: "IncidentReports");

            migrationBuilder.DropIndex(
                name: "IX_IncidentReports_UpdatedByUserId",
                table: "IncidentReports");

            migrationBuilder.DropIndex(
                name: "IX_AssetTypes_CreatedByUserId",
                table: "AssetTypes");

            migrationBuilder.DropIndex(
                name: "IX_AssetTypes_UpdatedByUserId",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OperationShifts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "OperationShifts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "IsCodeGenerated",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AssetModels");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AssetModels");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AssetAdditions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AssetAdditions");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "IncidentReports",
                newName: "UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_IncidentReports_CreatedByUserId",
                table: "IncidentReports",
                newName: "IX_IncidentReports_UpdatedById");

            migrationBuilder.AddColumn<long>(
                name: "CreatedById",
                table: "IncidentReports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_CreatedById",
                table: "IncidentReports",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentReports_Users_CreatedById",
                table: "IncidentReports",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentReports_Users_UpdatedById",
                table: "IncidentReports",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
