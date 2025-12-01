using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationShiftTaskStatusHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationTaskId = table.Column<long>(type: "bigint", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    RecordedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AdditionalData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStatusHistory_OperationTasks_OperationTaskId",
                        column: x => x.OperationTaskId,
                        principalTable: "OperationTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_EventType",
                table: "OperationShiftTaskStatusHistory",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_IsResolved",
                table: "OperationShiftTaskStatusHistory",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_OperationTaskId",
                table: "OperationShiftTaskStatusHistory",
                column: "OperationTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_OperationTaskId_EventType",
                table: "OperationShiftTaskStatusHistory",
                columns: new[] { "OperationTaskId", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_OperationTaskId_IsResolved",
                table: "OperationShiftTaskStatusHistory",
                columns: new[] { "OperationTaskId", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_Severity",
                table: "OperationShiftTaskStatusHistory",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_StartTime",
                table: "OperationShiftTaskStatusHistory",
                column: "StartTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationShiftTaskStatusHistory");
        }
    }
}
