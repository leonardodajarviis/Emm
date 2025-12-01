using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThayDoiLon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationShiftParameterReadings");

            migrationBuilder.DropTable(
                name: "OperationShiftTaskCheckpointValues");

            migrationBuilder.DropTable(
                name: "OperationShiftTaskStatusHistory");

            migrationBuilder.DropTable(
                name: "OperationShiftTaskCheckpoints");

            migrationBuilder.DropTable(
                name: "OperationShiftTasks");

            migrationBuilder.CreateTable(
                name: "ShiftLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationShiftId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftLogCheckpoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LinkedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftLogId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsWithAttachedMaterial = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftLogCheckpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftLogCheckpoints_ShiftLogs_ShiftLogId",
                        column: x => x.ShiftLogId,
                        principalTable: "ShiftLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftLogEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftLogId = table.Column<long>(type: "bigint", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftLogEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftLogEvents_ShiftLogs_ShiftLogId",
                        column: x => x.ShiftLogId,
                        principalTable: "ShiftLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftLogParameterReadings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftLogCheckpointLinkedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShiftLogId = table.Column<long>(type: "bigint", nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    AssetCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParameterId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParameterCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    StringValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReadingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftLogParameterReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftLogParameterReadings_ShiftLogs_ShiftLogId",
                        column: x => x.ShiftLogId,
                        principalTable: "ShiftLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogCheckpoints_ShiftLogId",
                table: "ShiftLogCheckpoints",
                column: "ShiftLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogEvents_EventType",
                table: "ShiftLogEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogEvents_ShiftLogId",
                table: "ShiftLogEvents",
                column: "ShiftLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogEvents_ShiftLogId_EventType",
                table: "ShiftLogEvents",
                columns: new[] { "ShiftLogId", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogEvents_StartTime",
                table: "ShiftLogEvents",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_AssetId",
                table: "ShiftLogParameterReadings",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_AssetId_ParameterId_ReadingTime",
                table: "ShiftLogParameterReadings",
                columns: new[] { "AssetId", "ParameterId", "ReadingTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_ParameterCode",
                table: "ShiftLogParameterReadings",
                column: "ParameterCode");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_ParameterId",
                table: "ShiftLogParameterReadings",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_ReadingTime",
                table: "ShiftLogParameterReadings",
                column: "ReadingTime");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_ShiftLogId",
                table: "ShiftLogParameterReadings",
                column: "ShiftLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogParameterReadings_ShiftLogId_AssetId",
                table: "ShiftLogParameterReadings",
                columns: new[] { "ShiftLogId", "AssetId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_OperationShiftId",
                table: "ShiftLogs",
                column: "OperationShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftLogCheckpoints");

            migrationBuilder.DropTable(
                name: "ShiftLogEvents");

            migrationBuilder.DropTable(
                name: "ShiftLogParameterReadings");

            migrationBuilder.DropTable(
                name: "ShiftLogs");

            migrationBuilder.CreateTable(
                name: "OperationShiftTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OperationShiftId = table.Column<long>(type: "bigint", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationShiftParameterReadings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OperationTaskId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParameterId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReadingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StringValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaskCheckpointId = table.Column<long>(type: "bigint", nullable: true),
                    TaskCheckpointLinkedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftParameterReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationShiftParameterReadings_OperationShiftTasks_OperationTaskId",
                        column: x => x.OperationTaskId,
                        principalTable: "OperationShiftTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationShiftTaskCheckpoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckpointTemplateId = table.Column<long>(type: "bigint", nullable: true),
                    IsWithAttachedMaterial = table.Column<bool>(type: "bit", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemId = table.Column<long>(type: "bigint", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OperationTaskId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftTaskCheckpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationShiftTaskCheckpoints_CheckpointTemplates_CheckpointTemplateId",
                        column: x => x.CheckpointTemplateId,
                        principalTable: "CheckpointTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationShiftTaskCheckpoints_OperationShiftTasks_OperationTaskId",
                        column: x => x.OperationTaskId,
                        principalTable: "OperationShiftTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationShiftTaskStatusHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    OperationTaskId = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftTaskStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationShiftTaskStatusHistory_OperationShiftTasks_OperationTaskId",
                        column: x => x.OperationTaskId,
                        principalTable: "OperationShiftTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationShiftTaskCheckpointValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckpointId = table.Column<long>(type: "bigint", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TemplateFieldId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_AssetId",
                table: "OperationShiftParameterReadings",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_AssetId_ParameterId_ReadingTime",
                table: "OperationShiftParameterReadings",
                columns: new[] { "AssetId", "ParameterId", "ReadingTime" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_OperationTaskId",
                table: "OperationShiftParameterReadings",
                column: "OperationTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_OperationTaskId_AssetId",
                table: "OperationShiftParameterReadings",
                columns: new[] { "OperationTaskId", "AssetId" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_ParameterCode",
                table: "OperationShiftParameterReadings",
                column: "ParameterCode");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_ParameterId",
                table: "OperationShiftParameterReadings",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftParameterReadings_ReadingTime",
                table: "OperationShiftParameterReadings",
                column: "ReadingTime");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskCheckpoints_CheckpointTemplateId",
                table: "OperationShiftTaskCheckpoints",
                column: "CheckpointTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskCheckpoints_OperationTaskId",
                table: "OperationShiftTaskCheckpoints",
                column: "OperationTaskId");

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

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTasks_OperationShiftId",
                table: "OperationShiftTasks",
                column: "OperationShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskStatusHistory_EventType",
                table: "OperationShiftTaskStatusHistory",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskStatusHistory_OperationTaskId",
                table: "OperationShiftTaskStatusHistory",
                column: "OperationTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskStatusHistory_OperationTaskId_EventType",
                table: "OperationShiftTaskStatusHistory",
                columns: new[] { "OperationTaskId", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftTaskStatusHistory_StartTime",
                table: "OperationShiftTaskStatusHistory",
                column: "StartTime");
        }
    }
}
