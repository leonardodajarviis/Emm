using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TrusedBigUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategories_Users_UpdatedByUserId",
                table: "AssetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_Users_UpdatedByUserId",
                table: "AssetModels");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTypes_Users_UpdatedByUserId",
                table: "AssetTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentReports_Users_CreatedByUserId",
                table: "IncidentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentReports_Users_UpdatedByUserId",
                table: "IncidentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterCatalogs_Users_UpdatedByUserId",
                table: "ParameterCatalogs");

            migrationBuilder.DropIndex(
                name: "IX_OperationShifts_CreatedAt",
                table: "OperationShifts");

            migrationBuilder.DropIndex(
                name: "IX_Assets_CreatedAt",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdditions_CreatedAt",
                table: "AssetAdditions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ParameterCatalogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OperationShifts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AssetModels");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AssetCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AssetAdditions");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "ShiftLogs",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Roles",
                newName: "Audit_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Roles",
                newName: "Audit_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Roles",
                newName: "Audit_ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Policies",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Permissions",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "ParameterCatalogs",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterCatalogs_UpdatedByUserId",
                table: "ParameterCatalogs",
                newName: "IX_ParameterCatalogs_ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "OperationShifts",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "MaintenancePlanDefinitions",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Locations",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Items",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "IncidentReports",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_IncidentReports_UpdatedByUserId",
                table: "IncidentReports",
                newName: "IX_IncidentReports_ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "AssetTypes",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetTypes_UpdatedByUserId",
                table: "AssetTypes",
                newName: "IX_AssetTypes_ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Assets",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "AssetModels",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetModels_UpdatedByUserId",
                table: "AssetModels",
                newName: "IX_AssetModels_ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "AssetCategories",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetCategories_UpdatedByUserId",
                table: "AssetCategories",
                newName: "IX_AssetCategories_ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "AssetAdditions",
                newName: "ModifiedByUserId");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "ShiftLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "ShiftLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Audit_CreatedByUserId",
                table: "Roles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Audit_CreatedAt",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "Audit_ModifiedAt",
                table: "Roles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Policies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Policies",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Policies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Permissions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Permissions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Permissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "ParameterCatalogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ParameterCatalogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "ParameterCatalogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "OperationShifts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "OperationShifts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "MaintenancePlanDefinitions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MaintenancePlanDefinitions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "MaintenancePlanDefinitions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Locations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Locations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Locations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Items",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Items",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "IncidentReports",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "IncidentReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "AssetTypes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AssetTypes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "AssetTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Assets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Assets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "AssetModels",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AssetModels",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "AssetModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "AssetCategories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AssetCategories",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "AssetCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "AssetAdditions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AssetAdditions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "AssetAdditions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_CreatedByUserId",
                table: "ShiftLogs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_ModifiedByUserId",
                table: "ShiftLogs",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CreatedByUserId",
                table: "Policies",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ModifiedByUserId",
                table: "Policies",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreatedByUserId",
                table: "Permissions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ModifiedByUserId",
                table: "Permissions",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_CreatedByUserId",
                table: "OperationShifts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_ModifiedByUserId",
                table: "OperationShifts",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_CreatedByUserId",
                table: "MaintenancePlanDefinitions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_ModifiedByUserId",
                table: "MaintenancePlanDefinitions",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CreatedByUserId",
                table: "Locations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ModifiedByUserId",
                table: "Locations",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedByUserId",
                table: "Items",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ModifiedByUserId",
                table: "Items",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CreatedByUserId",
                table: "Assets",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ModifiedByUserId",
                table: "Assets",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditions_CreatedByUserId",
                table: "AssetAdditions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditions_ModifiedByUserId",
                table: "AssetAdditions",
                column: "ModifiedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdditions_Users_CreatedByUserId",
                table: "AssetAdditions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAdditions_Users_ModifiedByUserId",
                table: "AssetAdditions",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategories_Users_ModifiedByUserId",
                table: "AssetCategories",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetModels_Users_ModifiedByUserId",
                table: "AssetModels",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Users_CreatedByUserId",
                table: "Assets",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Users_ModifiedByUserId",
                table: "Assets",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTypes_Users_ModifiedByUserId",
                table: "AssetTypes",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentReports_Users_CreatedByUserId",
                table: "IncidentReports",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidentReports_Users_ModifiedByUserId",
                table: "IncidentReports",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Users_CreatedByUserId",
                table: "Items",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Users_ModifiedByUserId",
                table: "Items",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Users_CreatedByUserId",
                table: "Locations",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Users_ModifiedByUserId",
                table: "Locations",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanDefinitions_Users_CreatedByUserId",
                table: "MaintenancePlanDefinitions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePlanDefinitions_Users_ModifiedByUserId",
                table: "MaintenancePlanDefinitions",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShifts_Users_CreatedByUserId",
                table: "OperationShifts",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShifts_Users_ModifiedByUserId",
                table: "OperationShifts",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterCatalogs_Users_ModifiedByUserId",
                table: "ParameterCatalogs",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_CreatedByUserId",
                table: "Permissions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_ModifiedByUserId",
                table: "Permissions",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Users_CreatedByUserId",
                table: "Policies",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_Users_ModifiedByUserId",
                table: "Policies",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogs_Users_CreatedByUserId",
                table: "ShiftLogs",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogs_Users_ModifiedByUserId",
                table: "ShiftLogs",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdditions_Users_CreatedByUserId",
                table: "AssetAdditions");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAdditions_Users_ModifiedByUserId",
                table: "AssetAdditions");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetCategories_Users_ModifiedByUserId",
                table: "AssetCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetModels_Users_ModifiedByUserId",
                table: "AssetModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Users_CreatedByUserId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Users_ModifiedByUserId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTypes_Users_ModifiedByUserId",
                table: "AssetTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentReports_Users_CreatedByUserId",
                table: "IncidentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidentReports_Users_ModifiedByUserId",
                table: "IncidentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Users_CreatedByUserId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Users_ModifiedByUserId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Users_CreatedByUserId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Users_ModifiedByUserId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanDefinitions_Users_CreatedByUserId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePlanDefinitions_Users_ModifiedByUserId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationShifts_Users_CreatedByUserId",
                table: "OperationShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationShifts_Users_ModifiedByUserId",
                table: "OperationShifts");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterCatalogs_Users_ModifiedByUserId",
                table: "ParameterCatalogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_CreatedByUserId",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_ModifiedByUserId",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Users_CreatedByUserId",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_Users_ModifiedByUserId",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogs_Users_CreatedByUserId",
                table: "ShiftLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogs_Users_ModifiedByUserId",
                table: "ShiftLogs");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogs_CreatedByUserId",
                table: "ShiftLogs");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogs_ModifiedByUserId",
                table: "ShiftLogs");

            migrationBuilder.DropIndex(
                name: "IX_Policies_CreatedByUserId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_ModifiedByUserId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_CreatedByUserId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_ModifiedByUserId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_OperationShifts_CreatedByUserId",
                table: "OperationShifts");

            migrationBuilder.DropIndex(
                name: "IX_OperationShifts_ModifiedByUserId",
                table: "OperationShifts");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanDefinitions_CreatedByUserId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePlanDefinitions_ModifiedByUserId",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_Locations_CreatedByUserId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_ModifiedByUserId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Items_CreatedByUserId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ModifiedByUserId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Assets_CreatedByUserId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_ModifiedByUserId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdditions_CreatedByUserId",
                table: "AssetAdditions");

            migrationBuilder.DropIndex(
                name: "IX_AssetAdditions_ModifiedByUserId",
                table: "AssetAdditions");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "Audit_ModifiedAt",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "ParameterCatalogs");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "OperationShifts");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "MaintenancePlanDefinitions");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "AssetModels");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "AssetCategories");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "AssetAdditions");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "ShiftLogs",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Audit_CreatedByUserId",
                table: "Roles",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Audit_CreatedAt",
                table: "Roles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Audit_ModifiedByUserId",
                table: "Roles",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Policies",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Permissions",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "ParameterCatalogs",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ParameterCatalogs_ModifiedByUserId",
                table: "ParameterCatalogs",
                newName: "IX_ParameterCatalogs_UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "OperationShifts",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "MaintenancePlanDefinitions",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Locations",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Items",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "IncidentReports",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_IncidentReports_ModifiedByUserId",
                table: "IncidentReports",
                newName: "IX_IncidentReports_UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "AssetTypes",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetTypes_ModifiedByUserId",
                table: "AssetTypes",
                newName: "IX_AssetTypes_UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Assets",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "AssetModels",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetModels_ModifiedByUserId",
                table: "AssetModels",
                newName: "IX_AssetModels_UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "AssetCategories",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetCategories_ModifiedByUserId",
                table: "AssetCategories",
                newName: "IX_AssetCategories_UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "AssetAdditions",
                newName: "UpdatedByUserId");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "ShiftLogs",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ShiftLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Roles",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Policies",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Policies",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Policies",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Permissions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Permissions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Permissions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "ParameterCatalogs",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ParameterCatalogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ParameterCatalogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "OperationShifts",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OperationShifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "MaintenancePlanDefinitions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "MaintenancePlanDefinitions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MaintenancePlanDefinitions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Locations",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Locations",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Locations",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Items",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "IncidentReports",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IncidentReports",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "AssetTypes",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AssetTypes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AssetTypes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "Assets",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Assets",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "AssetModels",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AssetModels",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AssetModels",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "AssetCategories",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AssetCategories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AssetCategories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<long>(
                name: "CreatedByUserId",
                table: "AssetAdditions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AssetAdditions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AssetAdditions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_CreatedAt",
                table: "OperationShifts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CreatedAt",
                table: "Assets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditions_CreatedAt",
                table: "AssetAdditions",
                column: "CreatedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCategories_Users_UpdatedByUserId",
                table: "AssetCategories",
                column: "UpdatedByUserId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterCatalogs_Users_UpdatedByUserId",
                table: "ParameterCatalogs",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
