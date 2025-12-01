using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Suju : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperationShifts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    PrimaryOperatorId = table.Column<long>(type: "bigint", nullable: false),
                    PrimaryOperatorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScheduledStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUnitLevels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUnitLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attempt = table.Column<int>(type: "int", nullable: false),
                    LockId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParameterCatalogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterCatalogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SequenceNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumberLength = table.Column<int>(type: "int", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrentNumber = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenceNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AssetCategoryId = table.Column<long>(type: "bigint", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetTypes_AssetCategories_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperationLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationShiftId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationShiftId1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationLogs_OperationShifts_OperationShiftId",
                        column: x => x.OperationShiftId,
                        principalTable: "OperationShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperationLogs_OperationShifts_OperationShiftId1",
                        column: x => x.OperationShiftId1,
                        principalTable: "OperationShifts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OperationShiftAssets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationShiftId = table.Column<long>(type: "bigint", nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    AssetCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    AssignedOperatorId = table.Column<long>(type: "bigint", nullable: true),
                    AssignedOperatorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationShiftId1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationShiftAssets_OperationShifts_OperationShiftId",
                        column: x => x.OperationShiftId,
                        principalTable: "OperationShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperationShiftAssets_OperationShifts_OperationShiftId1",
                        column: x => x.OperationShiftId1,
                        principalTable: "OperationShifts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OperationTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationShiftId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationShiftId1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationTasks_OperationShifts_OperationShiftId",
                        column: x => x.OperationShiftId,
                        principalTable: "OperationShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperationTasks_OperationShifts_OperationShiftId1",
                        column: x => x.OperationShiftId1,
                        principalTable: "OperationShifts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    OrganizationUnitLevelId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationUnits_OrganizationUnitLevels_OrganizationUnitLevelId",
                        column: x => x.OrganizationUnitLevelId,
                        principalTable: "OrganizationUnitLevels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetModels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ParentId = table.Column<long>(type: "bigint", maxLength: 50, nullable: true),
                    AssetCategoryId = table.Column<long>(type: "bigint", maxLength: 50, nullable: true),
                    AssetTypeId = table.Column<long>(type: "bigint", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetModels_AssetCategories_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AssetModels_AssetModels_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AssetModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetModels_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssetTypeParameters",
                columns: table => new
                {
                    AssetTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypeParameters", x => new { x.AssetTypeId, x.ParameterId });
                    table.ForeignKey(
                        name: "FK_AssetTypeParameters_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetTypeParameters_ParameterCatalogs_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "ParameterCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParameterReadings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationTaskId = table.Column<long>(type: "bigint", nullable: false),
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
                    ReadBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationTaskId1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParameterReadings_OperationTasks_OperationTaskId",
                        column: x => x.OperationTaskId,
                        principalTable: "OperationTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParameterReadings_OperationTasks_OperationTaskId1",
                        column: x => x.OperationTaskId1,
                        principalTable: "OperationTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskCheckpoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationTaskId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationTaskId1 = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCheckpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCheckpoints_OperationTasks_OperationTaskId",
                        column: x => x.OperationTaskId,
                        principalTable: "OperationTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskCheckpoints_OperationTasks_OperationTaskId1",
                        column: x => x.OperationTaskId1,
                        principalTable: "OperationTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetAdditions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    DecisionNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DecisionDate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAdditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetAdditions_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetAdditions_OrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "OrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_OrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "OrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetModelParameters",
                columns: table => new
                {
                    AssetModelId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetModelParameters", x => new { x.AssetModelId, x.ParameterId });
                    table.ForeignKey(
                        name: "FK_AssetModelParameters_AssetModels_AssetModelId",
                        column: x => x.AssetModelId,
                        principalTable: "AssetModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetModelParameters_ParameterCatalogs_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "ParameterCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetModelId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetModels_AssetModelId",
                        column: x => x.AssetModelId,
                        principalTable: "AssetModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePlanDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssetModelId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterId = table.Column<long>(type: "bigint", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Min = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Max = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlanDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanDefinitions_AssetModels_AssetModelId",
                        column: x => x.AssetModelId,
                        principalTable: "AssetModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanDefinitions_ParameterCatalogs_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "ParameterCatalogs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetAdditionLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetAdditionId = table.Column<long>(type: "bigint", nullable: false),
                    AssetModelId = table.Column<long>(type: "bigint", nullable: false),
                    AssetCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAdditionLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetAdditionLines_AssetAdditions_AssetAdditionId",
                        column: x => x.AssetAdditionId,
                        principalTable: "AssetAdditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetAdditionLines_AssetModels_AssetModelId",
                        column: x => x.AssetModelId,
                        principalTable: "AssetModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetParameters",
                columns: table => new
                {
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetParameters", x => new { x.AssetId, x.ParameterId });
                    table.ForeignKey(
                        name: "FK_AssetParameters_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetParameters_ParameterCatalogs_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "ParameterCatalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePlanInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    ParameterId = table.Column<long>(type: "bigint", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Min = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Max = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlanInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanInstances_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanInstances_ParameterCatalogs_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "ParameterCatalogs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePlanJobStepDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlanJobStepDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitions_MaintenancePlanDefinitionId",
                        column: x => x.MaintenancePlanDefinitionId,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanJobStepDefinitions_OrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "OrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePlanJobStepInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenancePlanInstanceId = table.Column<long>(type: "bigint", nullable: false),
                    OrganizationUnitId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlanJobStepInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanJobStepInstances_MaintenancePlanInstances_MaintenancePlanInstanceId",
                        column: x => x.MaintenancePlanInstanceId,
                        principalTable: "MaintenancePlanInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanJobStepInstances_OrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "OrganizationUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditionLines_AssetAdditionId",
                table: "AssetAdditionLines",
                column: "AssetAdditionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditionLines_AssetAdditionId_AssetCode",
                table: "AssetAdditionLines",
                columns: new[] { "AssetAdditionId", "AssetCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditionLines_AssetCode",
                table: "AssetAdditionLines",
                column: "AssetCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditionLines_AssetModelId",
                table: "AssetAdditionLines",
                column: "AssetModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditions_Code",
                table: "AssetAdditions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditions_CreatedAt",
                table: "AssetAdditions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditions_LocationId",
                table: "AssetAdditions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditions_OrganizationUnitId",
                table: "AssetAdditions",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_Code",
                table: "AssetCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_IsActive",
                table: "AssetCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_Name",
                table: "AssetCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetModelParameters_ParameterId",
                table: "AssetModelParameters",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_AssetCategoryId",
                table: "AssetModels",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_AssetTypeId",
                table: "AssetModels",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_Code",
                table: "AssetModels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_IsActive",
                table: "AssetModels",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_Name",
                table: "AssetModels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_ParentId",
                table: "AssetModels",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParameters_ParameterId",
                table: "AssetParameters",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetModelId",
                table: "Assets",
                column: "AssetModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Code",
                table: "Assets",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CreatedAt",
                table: "Assets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Name",
                table: "Assets",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypeParameters_ParameterId",
                table: "AssetTypeParameters",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_AssetCategoryId",
                table: "AssetTypes",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_Code",
                table: "AssetTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_IsActive",
                table: "AssetTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_Name_AssetCategoryId",
                table: "AssetTypes",
                columns: new[] { "Name", "AssetCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Code",
                table: "Employees",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_OrganizationUnitId",
                table: "Employees",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Code",
                table: "Locations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_AssetModelId_IsActive",
                table: "MaintenancePlanDefinitions",
                columns: new[] { "AssetModelId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_ParameterId",
                table: "MaintenancePlanDefinitions",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanInstances_AssetId",
                table: "MaintenancePlanInstances",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanInstances_ParameterId",
                table: "MaintenancePlanInstances",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitionId_Order",
                table: "MaintenancePlanJobStepDefinitions",
                columns: new[] { "MaintenancePlanDefinitionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepDefinitions_OrganizationUnitId",
                table: "MaintenancePlanJobStepDefinitions",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepInstances_MaintenancePlanInstanceId",
                table: "MaintenancePlanJobStepInstances",
                column: "MaintenancePlanInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepInstances_OrganizationUnitId",
                table: "MaintenancePlanJobStepInstances",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_CreatedAt",
                table: "OperationLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_Level",
                table: "OperationLogs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_OperationShiftId",
                table: "OperationLogs",
                column: "OperationShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_OperationShiftId_Timestamp",
                table: "OperationLogs",
                columns: new[] { "OperationShiftId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_OperationShiftId1",
                table: "OperationLogs",
                column: "OperationShiftId1");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_Timestamp",
                table: "OperationLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_Type",
                table: "OperationLogs",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OperationLogs_Type_Level",
                table: "OperationLogs",
                columns: new[] { "Type", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_AssetCode",
                table: "OperationShiftAssets",
                column: "AssetCode");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_AssetId",
                table: "OperationShiftAssets",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_AssignedOperatorId",
                table: "OperationShiftAssets",
                column: "AssignedOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_CreatedAt",
                table: "OperationShiftAssets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_OperationShiftId",
                table: "OperationShiftAssets",
                column: "OperationShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_OperationShiftId_AssetId",
                table: "OperationShiftAssets",
                columns: new[] { "OperationShiftId", "AssetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_OperationShiftId1",
                table: "OperationShiftAssets",
                column: "OperationShiftId1");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_Status",
                table: "OperationShiftAssets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_Code",
                table: "OperationShifts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_CreatedAt",
                table: "OperationShifts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_LocationId",
                table: "OperationShifts",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_PrimaryOperatorId",
                table: "OperationShifts",
                column: "PrimaryOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_ScheduledStartTime",
                table: "OperationShifts",
                column: "ScheduledStartTime");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_Status",
                table: "OperationShifts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTasks_CreatedAt",
                table: "OperationTasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTasks_OperationShiftId",
                table: "OperationTasks",
                column: "OperationShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTasks_OperationShiftId_Order",
                table: "OperationTasks",
                columns: new[] { "OperationShiftId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationTasks_OperationShiftId1",
                table: "OperationTasks",
                column: "OperationShiftId1");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTasks_Order",
                table: "OperationTasks",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTasks_Status",
                table: "OperationTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTasks_Type",
                table: "OperationTasks",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnitLevels_Name",
                table: "OrganizationUnitLevels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnits_Code",
                table: "OrganizationUnits",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnits_IsActive",
                table: "OrganizationUnits",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnits_Name",
                table: "OrganizationUnits",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnits_OrganizationUnitLevelId",
                table: "OrganizationUnits",
                column: "OrganizationUnitLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAt_CreatedAt",
                table: "OutboxMessages",
                columns: new[] { "ProcessedAt", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterCatalogs_Code",
                table: "ParameterCatalogs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_AssetId",
                table: "ParameterReadings",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_AssetId_ParameterId_ReadingTime",
                table: "ParameterReadings",
                columns: new[] { "AssetId", "ParameterId", "ReadingTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_CreatedAt",
                table: "ParameterReadings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_OperationTaskId",
                table: "ParameterReadings",
                column: "OperationTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_OperationTaskId_AssetId",
                table: "ParameterReadings",
                columns: new[] { "OperationTaskId", "AssetId" });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_OperationTaskId1",
                table: "ParameterReadings",
                column: "OperationTaskId1");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_ParameterCode",
                table: "ParameterReadings",
                column: "ParameterCode");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_ParameterId",
                table: "ParameterReadings",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_ReadingTime",
                table: "ParameterReadings",
                column: "ReadingTime");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_Status",
                table: "ParameterReadings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterReadings_Type",
                table: "ParameterReadings",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_SequenceNumbers_Prefix_NumberLength_SequenceNumber",
                table: "SequenceNumbers",
                columns: new[] { "Prefix", "TableName", "NumberLength" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckpoints_CompletedAt",
                table: "TaskCheckpoints",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckpoints_CreatedAt",
                table: "TaskCheckpoints",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckpoints_IsRequired",
                table: "TaskCheckpoints",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckpoints_OperationTaskId",
                table: "TaskCheckpoints",
                column: "OperationTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckpoints_OperationTaskId1",
                table: "TaskCheckpoints",
                column: "OperationTaskId1");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckpoints_Status",
                table: "TaskCheckpoints",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetAdditionLines");

            migrationBuilder.DropTable(
                name: "AssetModelParameters");

            migrationBuilder.DropTable(
                name: "AssetParameters");

            migrationBuilder.DropTable(
                name: "AssetTypeParameters");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "MaintenancePlanJobStepDefinitions");

            migrationBuilder.DropTable(
                name: "MaintenancePlanJobStepInstances");

            migrationBuilder.DropTable(
                name: "OperationLogs");

            migrationBuilder.DropTable(
                name: "OperationShiftAssets");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "ParameterReadings");

            migrationBuilder.DropTable(
                name: "SequenceNumbers");

            migrationBuilder.DropTable(
                name: "TaskCheckpoints");

            migrationBuilder.DropTable(
                name: "AssetAdditions");

            migrationBuilder.DropTable(
                name: "MaintenancePlanDefinitions");

            migrationBuilder.DropTable(
                name: "MaintenancePlanInstances");

            migrationBuilder.DropTable(
                name: "OperationTasks");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "OrganizationUnits");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "ParameterCatalogs");

            migrationBuilder.DropTable(
                name: "OperationShifts");

            migrationBuilder.DropTable(
                name: "OrganizationUnitLevels");

            migrationBuilder.DropTable(
                name: "AssetModels");

            migrationBuilder.DropTable(
                name: "AssetTypes");

            migrationBuilder.DropTable(
                name: "AssetCategories");
        }
    }
}
