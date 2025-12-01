using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTaskCheckpoints_CompletedAt",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTaskCheckpoints_IsRequired",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropIndex(
                name: "IX_OperationShiftTaskCheckpoints_Status",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "CompletedBy",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.RenameColumn(
                name: "IsRequired",
                table: "OperationShiftTaskCheckpoints",
                newName: "IsWithAttachedMaterial");

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "OperationShiftTaskCheckpoints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ItemId",
                table: "OperationShiftTaskCheckpoints",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "OperationShiftTaskCheckpoints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LinkedId",
                table: "OperationShiftTaskCheckpoints",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "OperationShiftTaskCheckpoints",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "OperationShiftTaskCheckpoints",
                type: "nvarchar(1200)",
                maxLength: 1200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "TaskCheckpointId",
                table: "OperationShiftParameterReadings",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaskCheckpointLinkedId",
                table: "OperationShiftParameterReadings",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "LinkedId",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropColumn(
                name: "TaskCheckpointId",
                table: "OperationShiftParameterReadings");

            migrationBuilder.DropColumn(
                name: "TaskCheckpointLinkedId",
                table: "OperationShiftParameterReadings");

            migrationBuilder.RenameColumn(
                name: "IsWithAttachedMaterial",
                table: "OperationShiftTaskCheckpoints",
                newName: "IsRequired");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "OperationShiftTaskCheckpoints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompletedBy",
                table: "OperationShiftTaskCheckpoints",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OperationShiftTaskCheckpoints",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "OperationShiftTaskCheckpoints",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OperationShiftTaskCheckpoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskCheckpoints_CompletedAt",
                table: "OperationShiftTaskCheckpoints",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskCheckpoints_IsRequired",
                table: "OperationShiftTaskCheckpoints",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskCheckpoints_Status",
                table: "OperationShiftTaskCheckpoints",
                column: "Status");
        }
    }
}
