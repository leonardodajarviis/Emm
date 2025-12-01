using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "OperationShiftTaskCheckpoints",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "OperationShiftTaskCheckpoints",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<long>(
                name: "CheckpointTemplateId",
                table: "OperationShiftTaskCheckpoints",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CheckpointTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckpointTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationShiftTaskCheckpointValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckpointId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateFieldId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DisplayText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftTaskCheckpointValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationShiftTaskCheckpointValues_OperationShiftTaskCheckpoints_CheckpointId",
                        column: x => x.CheckpointId,
                        principalTable: "OperationShiftTaskCheckpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckpointTemplateAssetCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckpointTemplateId = table.Column<long>(type: "bigint", nullable: false),
                    AssetCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
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
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FieldType = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DefaultValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValidationRules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MasterDataTypeId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
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
                name: "IX_OperationShiftTaskCheckpoints_CheckpointTemplateId",
                table: "OperationShiftTaskCheckpoints",
                column: "CheckpointTemplateId");

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

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointValues_CheckpointId",
                table: "OperationShiftTaskCheckpointValues",
                column: "CheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointValues_CheckpointId_TemplateFieldId",
                table: "OperationShiftTaskCheckpointValues",
                columns: new[] { "CheckpointId", "TemplateFieldId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointValues_CreatedAt",
                table: "OperationShiftTaskCheckpointValues",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointValues_DisplayText",
                table: "OperationShiftTaskCheckpointValues",
                column: "DisplayText");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointValues_TemplateFieldId",
                table: "OperationShiftTaskCheckpointValues",
                column: "TemplateFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckpointValues_Value",
                table: "OperationShiftTaskCheckpointValues",
                column: "Value");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationShiftTaskCheckpoints_CheckpointTemplates_CheckpointTemplateId",
                table: "OperationShiftTaskCheckpoints",
                column: "CheckpointTemplateId",
                principalTable: "CheckpointTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationShiftTaskCheckpoints_CheckpointTemplates_CheckpointTemplateId",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropTable(
                name: "CheckpointTemplateAssetCategories");

            migrationBuilder.DropTable(
                name: "CheckpointTemplateFields");

            migrationBuilder.DropTable(
                name: "OperationShiftTaskCheckpointValues");

            migrationBuilder.DropTable(
                name: "CheckpointTemplates");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTaskCheckpoints_CheckpointTemplateId",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "CheckpointTemplateId",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "OperationShiftTaskCheckpoints",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "OperationShiftTaskCheckpoints",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");
        }
    }
}
