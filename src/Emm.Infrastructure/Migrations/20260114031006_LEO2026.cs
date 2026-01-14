using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LEO2026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasures_IsActive_UnitType",
                table: "UnitOfMeasures");

            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasures_UnitType",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "UnitType",
                table: "UnitOfMeasures");

            migrationBuilder.RenameColumn(
                name: "WarehouseIssueSlipId",
                table: "ShiftLogItems",
                newName: "GoodsIssueLineId");

            migrationBuilder.AddColumn<Guid>(
                name: "GoodsIssueId",
                table: "ShiftLogItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IncidentId",
                table: "ShiftLogEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckpointLogEnabled",
                table: "OperationShiftAssets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitOfMeasureCategoryId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "GoodsIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Audit_CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Audit_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Audit_ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsIssues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsIssueLine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoodsIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitOfMeasureCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfMeasureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsIssueLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsIssueLine_GoodsIssues_GoodsIssueId",
                        column: x => x.GoodsIssueId,
                        principalTable: "GoodsIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogItems_GoodsIssueId",
                table: "ShiftLogItems",
                column: "GoodsIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogItems_GoodsIssueLineId",
                table: "ShiftLogItems",
                column: "GoodsIssueLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogEvents_IncidentId",
                table: "ShiftLogEvents",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsIssueLine_GoodsIssueId",
                table: "GoodsIssueLine",
                column: "GoodsIssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogEvents_IncidentReports_IncidentId",
                table: "ShiftLogEvents",
                column: "IncidentId",
                principalTable: "IncidentReports",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogItems_GoodsIssueLine_GoodsIssueLineId",
                table: "ShiftLogItems",
                column: "GoodsIssueLineId",
                principalTable: "GoodsIssueLine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftLogItems_GoodsIssues_GoodsIssueId",
                table: "ShiftLogItems",
                column: "GoodsIssueId",
                principalTable: "GoodsIssues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogEvents_IncidentReports_IncidentId",
                table: "ShiftLogEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogItems_GoodsIssueLine_GoodsIssueLineId",
                table: "ShiftLogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftLogItems_GoodsIssues_GoodsIssueId",
                table: "ShiftLogItems");

            migrationBuilder.DropTable(
                name: "GoodsIssueLine");

            migrationBuilder.DropTable(
                name: "GoodsIssues");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogItems_GoodsIssueId",
                table: "ShiftLogItems");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogItems_GoodsIssueLineId",
                table: "ShiftLogItems");

            migrationBuilder.DropIndex(
                name: "IX_ShiftLogEvents_IncidentId",
                table: "ShiftLogEvents");

            migrationBuilder.DropColumn(
                name: "GoodsIssueId",
                table: "ShiftLogItems");

            migrationBuilder.DropColumn(
                name: "IncidentId",
                table: "ShiftLogEvents");

            migrationBuilder.DropColumn(
                name: "IsCheckpointLogEnabled",
                table: "OperationShiftAssets");

            migrationBuilder.RenameColumn(
                name: "GoodsIssueLineId",
                table: "ShiftLogItems",
                newName: "WarehouseIssueSlipId");

            migrationBuilder.AddColumn<int>(
                name: "UnitType",
                table: "UnitOfMeasures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitOfMeasureCategoryId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_IsActive_UnitType",
                table: "UnitOfMeasures",
                columns: new[] { "IsActive", "UnitType" });

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_UnitType",
                table: "UnitOfMeasures",
                column: "UnitType");
        }
    }
}
