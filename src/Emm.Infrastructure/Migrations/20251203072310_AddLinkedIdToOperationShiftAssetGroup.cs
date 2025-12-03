using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkedIdToOperationShiftAssetGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckpointTemplateAssetCategories");

            migrationBuilder.DropTable(
                name: "CheckpointTemplateFields");

            migrationBuilder.DropTable(
                name: "CheckpointTemplates");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_ProcessedAt_CreatedAt",
                table: "OutboxMessages");

            migrationBuilder.AddColumn<int>(
                name: "LogOrder",
                table: "ShiftLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLooked",
                table: "ShiftLogParameterReadings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OutboxMessages",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "LockId",
                table: "OutboxMessages",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Attempt",
                table: "OutboxMessages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "AssetGroupId",
                table: "OperationShiftAssets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OperationShiftAssetGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LinkedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationShiftId = table.Column<long>(type: "bigint", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftAssetGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationShiftAssetGroups_OperationShifts_OperationShiftId",
                        column: x => x.OperationShiftId,
                        principalTable: "OperationShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftLogAssets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftLogId = table.Column<long>(type: "bigint", nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    AssetCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
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
                name: "IX_OutboxMessages_CreatedAt",
                table: "OutboxMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_LockId",
                table: "OutboxMessages",
                column: "LockId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Processing",
                table: "OutboxMessages",
                columns: new[] { "ProcessedAt", "LockedUntil", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_AssetGroupId",
                table: "OperationShiftAssets",
                column: "AssetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetGroups_LinkedId",
                table: "OperationShiftAssetGroups",
                column: "LinkedId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetGroups_OperationShiftId",
                table: "OperationShiftAssetGroups",
                column: "OperationShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetGroups_OperationShiftId_DisplayOrder",
                table: "OperationShiftAssetGroups",
                columns: new[] { "OperationShiftId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetGroups_OperationShiftId_LinkedId",
                table: "OperationShiftAssetGroups",
                columns: new[] { "OperationShiftId", "LinkedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetGroups_Role",
                table: "OperationShiftAssetGroups",
                column: "Role");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationShiftAssetGroups");

            migrationBuilder.DropTable(
                name: "ShiftLogAssets");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_CreatedAt",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_LockId",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_Processing",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftAssets_AssetGroupId",
                table: "OperationShiftAssets");

            migrationBuilder.DropColumn(
                name: "LogOrder",
                table: "ShiftLogs");

            migrationBuilder.DropColumn(
                name: "IsLooked",
                table: "ShiftLogParameterReadings");

            migrationBuilder.DropColumn(
                name: "AssetGroupId",
                table: "OperationShiftAssets");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OutboxMessages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "LockId",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Attempt",
                table: "OutboxMessages",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CheckpointTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckpointTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckpointTemplateAssetCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CheckpointTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckpointTemplateAssetCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckpointTemplateAssetCategories_CheckpointTemplates_CheckpointTemplateId",
                        column: x => x.CheckpointTemplateId,
                        principalTable: "CheckpointTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckpointTemplateFields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckpointTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DefaultValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FieldType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MasterDataTypeId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ValidationRules = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckpointTemplateFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckpointTemplateFields_CheckpointTemplates_CheckpointTemplateId",
                        column: x => x.CheckpointTemplateId,
                        principalTable: "CheckpointTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAt_CreatedAt",
                table: "OutboxMessages",
                columns: new[] { "ProcessedAt", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateAssetCategories_AssignedAt",
                table: "CheckpointTemplateAssetCategories",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateAssetCategories_CategoryId",
                table: "CheckpointTemplateAssetCategories",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateAssetCategories_IsActive",
                table: "CheckpointTemplateAssetCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateAssetCategories_TemplateId",
                table: "CheckpointTemplateAssetCategories",
                column: "CheckpointTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateAssetCategories_TemplateId_CategoryId",
                table: "CheckpointTemplateAssetCategories",
                columns: new[] { "CheckpointTemplateId", "AssetCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateFields_FieldType",
                table: "CheckpointTemplateFields",
                column: "FieldType");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateFields_IsActive",
                table: "CheckpointTemplateFields",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateFields_MasterDataTypeId",
                table: "CheckpointTemplateFields",
                column: "MasterDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateFields_Order",
                table: "CheckpointTemplateFields",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateFields_TemplateId",
                table: "CheckpointTemplateFields",
                column: "CheckpointTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplateFields_TemplateId_Code",
                table: "CheckpointTemplateFields",
                columns: new[] { "CheckpointTemplateId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplates_Code",
                table: "CheckpointTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplates_CreatedAt",
                table: "CheckpointTemplates",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplates_IsActive",
                table: "CheckpointTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointTemplates_Version",
                table: "CheckpointTemplates",
                column: "Version");
        }
    }
}
