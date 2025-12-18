using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationUnitLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Type = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attempt = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LockId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Audit_CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Audit_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Audit_ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Audit_ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SequenceNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    NumberLength = table.Column<int>(type: "int", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CurrentNumber = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenceNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnitType = table.Column<int>(type: "int", nullable: false),
                    BaseUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConversionFactor = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitOfMeasures_UnitOfMeasures_BaseUnitId",
                        column: x => x.BaseUnitId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Subfolder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationUnitLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "AssetCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsCodeGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetCategories_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetCategories_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_UnitOfMeasures_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locations_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperationShifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCheckpointLogEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ScheduledStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationShifts_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OperationShifts_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParameterCatalogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCodeGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterCatalogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParameterCatalogs_UnitOfMeasures_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParameterCatalogs_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParameterCatalogs_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Permissions_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResourceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ConditionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Policies_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Policies_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShiftLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogOrder = table.Column<int>(type: "int", nullable: false),
                    OperationShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftLogs_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftLogs_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessTokenJti = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RefreshTokenJti = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccessTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsCodeGenerated = table.Column<bool>(type: "bit", nullable: false),
                    AssetCategoryId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_AssetTypes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetTypes_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetAdditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecisionNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_AssetAdditions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAdditions_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperationShiftAssetBoxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoxName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationShiftAssetBoxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationShiftAssetBoxes_OperationShifts_OperationShiftId",
                        column: x => x.OperationShiftId,
                        principalTable: "OperationShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationShiftAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    AssetBoxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AssignedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => new { x.UserId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftLogCheckpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "ShiftLogItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssetCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitOfMeasureName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftLogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftLogItems_ShiftLogs_ShiftLogId",
                        column: x => x.ShiftLogId,
                        principalTable: "ShiftLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftLogParameterReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftLogCheckpointLinkedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShiftLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParameterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParameterCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    StringValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReadingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsLooked = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "AssetModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCodeGenerated = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: true),
                    AssetCategoryId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: true),
                    AssetTypeId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ThumbnailFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_AssetModels_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetModels_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetTypeParameters",
                columns: table => new
                {
                    AssetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "AssetAdditionLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetAdditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "AssetModelImages",
                columns: table => new
                {
                    AssetModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetModelImages", x => new { x.AssetModelId, x.FileId });
                    table.ForeignKey(
                        name: "FK_AssetModelImages_AssetModels_AssetModelId",
                        column: x => x.AssetModelId,
                        principalTable: "AssetModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetModelParameters",
                columns: table => new
                {
                    AssetModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsMaintenanceParameter = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetModelCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetModelName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetAdditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsLooked = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetAdditions_AssetAdditionId",
                        column: x => x.AssetAdditionId,
                        principalTable: "AssetAdditions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_AssetModels_AssetModelId",
                        column: x => x.AssetModelId,
                        principalTable: "AssetModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_OrganizationUnits_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "OrganizationUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePlanDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssetModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanType = table.Column<int>(type: "int", nullable: false),
                    RRule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                        name: "FK_MaintenancePlanDefinitions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanDefinitions_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetParameters",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParameterUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueToMaintenance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                name: "IncidentReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncidentReports_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePlanJobStepDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenancePlanDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
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
                name: "MaintenancePlanRequiredItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenancePlanDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlanRequiredItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePlanRequiredItems_MaintenancePlanDefinitions_MaintenancePlanDefinitionId",
                        column: x => x.MaintenancePlanDefinitionId,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParameterBasedMaintenanceTriggers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenancePlanDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TriggerValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TriggerCondition = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterBasedMaintenanceTriggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitions_MaintenancePlanDefinitionId",
                        column: x => x.MaintenancePlanDefinitionId,
                        principalTable: "MaintenancePlanDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_AssetAdditions_CreatedByUserId",
                table: "AssetAdditions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditions_LocationId",
                table: "AssetAdditions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAdditions_ModifiedByUserId",
                table: "AssetAdditions",
                column: "ModifiedByUserId");

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
                name: "IX_AssetCategories_CreatedByUserId",
                table: "AssetCategories",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_IsActive",
                table: "AssetCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_ModifiedByUserId",
                table: "AssetCategories",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_Name",
                table: "AssetCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetModelImages_FileId",
                table: "AssetModelImages",
                column: "FileId",
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
                name: "IX_AssetModels_CreatedByUserId",
                table: "AssetModels",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_IsActive",
                table: "AssetModels",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_ModifiedByUserId",
                table: "AssetModels",
                column: "ModifiedByUserId");

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
                name: "IX_Assets_AssetAdditionId",
                table: "Assets",
                column: "AssetAdditionId");

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
                name: "IX_Assets_CreatedByUserId",
                table: "Assets",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_DisplayName",
                table: "Assets",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_LocationId",
                table: "Assets",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ModifiedByUserId",
                table: "Assets",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_OrganizationUnitId",
                table: "Assets",
                column: "OrganizationUnitId");

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
                name: "IX_AssetTypes_CreatedByUserId",
                table: "AssetTypes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_IsActive",
                table: "AssetTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_ModifiedByUserId",
                table: "AssetTypes",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_Name_AssetCategoryId",
                table: "AssetTypes",
                columns: new[] { "Name", "AssetCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_AssetId",
                table: "IncidentReports",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_Code",
                table: "IncidentReports",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_CreatedByUserId",
                table: "IncidentReports",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_ModifiedByUserId",
                table: "IncidentReports",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Code",
                table: "Items",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedByUserId",
                table: "Items",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ModifiedByUserId",
                table: "Items",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UnitOfMeasureId",
                table: "Items",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Code",
                table: "Locations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CreatedByUserId",
                table: "Locations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ModifiedByUserId",
                table: "Locations",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_AssetModelId_IsActive_PlanType",
                table: "MaintenancePlanDefinitions",
                columns: new[] { "AssetModelId", "IsActive", "PlanType" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_CreatedByUserId",
                table: "MaintenancePlanDefinitions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanDefinitions_ModifiedByUserId",
                table: "MaintenancePlanDefinitions",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitionId_Order",
                table: "MaintenancePlanJobStepDefinitions",
                columns: new[] { "MaintenancePlanDefinitionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanJobStepDefinitions_OrganizationUnitId",
                table: "MaintenancePlanJobStepDefinitions",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanRequiredItems_ItemId",
                table: "MaintenancePlanRequiredItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePlanRequiredItems_MaintenancePlanDefinitionId",
                table: "MaintenancePlanRequiredItems",
                column: "MaintenancePlanDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetBoxes_OperationShiftId",
                table: "OperationShiftAssetBoxes",
                column: "OperationShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetBoxes_OperationShiftId_BoxName_Unique",
                table: "OperationShiftAssetBoxes",
                columns: new[] { "OperationShiftId", "BoxName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetBoxes_OperationShiftId_DisplayOrder",
                table: "OperationShiftAssetBoxes",
                columns: new[] { "OperationShiftId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssetBoxes_Role",
                table: "OperationShiftAssetBoxes",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_AssetBoxId",
                table: "OperationShiftAssets",
                column: "AssetBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_AssetCode",
                table: "OperationShiftAssets",
                column: "AssetCode");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShiftAssets_AssetId",
                table: "OperationShiftAssets",
                column: "AssetId");

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
                name: "IX_OperationShiftAssets_Status",
                table: "OperationShiftAssets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_Code",
                table: "OperationShifts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_CreatedByUserId",
                table: "OperationShifts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_ModifiedByUserId",
                table: "OperationShifts",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_OrganizationUnitId",
                table: "OperationShifts",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_PrimaryUserId",
                table: "OperationShifts",
                column: "PrimaryUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_ScheduledStartTime",
                table: "OperationShifts",
                column: "ScheduledStartTime");

            migrationBuilder.CreateIndex(
                name: "IX_OperationShifts_Status",
                table: "OperationShifts",
                column: "Status");

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
                name: "IX_ParameterBasedMaintenanceTriggers_IsActive",
                table: "ParameterBasedMaintenanceTriggers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitionId",
                table: "ParameterBasedMaintenanceTriggers",
                column: "MaintenancePlanDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_MaintenancePlanDefinitionId_ParameterId_IsActive",
                table: "ParameterBasedMaintenanceTriggers",
                columns: new[] { "MaintenancePlanDefinitionId", "ParameterId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_ParameterId",
                table: "ParameterBasedMaintenanceTriggers",
                column: "ParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_ParameterId_IsActive",
                table: "ParameterBasedMaintenanceTriggers",
                columns: new[] { "ParameterId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ParameterBasedMaintenanceTriggers_TriggerCondition",
                table: "ParameterBasedMaintenanceTriggers",
                column: "TriggerCondition");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterCatalogs_Code",
                table: "ParameterCatalogs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterCatalogs_CreatedByUserId",
                table: "ParameterCatalogs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterCatalogs_ModifiedByUserId",
                table: "ParameterCatalogs",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterCatalogs_UnitOfMeasureId",
                table: "ParameterCatalogs",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Category",
                table: "Permissions",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreatedByUserId",
                table: "Permissions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ModifiedByUserId",
                table: "Permissions",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Resource",
                table: "Permissions",
                column: "Resource");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_Code",
                table: "Policies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CreatedByUserId",
                table: "Policies",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_IsActive_Priority",
                table: "Policies",
                columns: new[] { "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ModifiedByUserId",
                table: "Policies",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ResourceType",
                table: "Policies",
                column: "ResourceType");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Code",
                table: "Roles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsActive",
                table: "Roles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UX_SequenceNumbers_Prefix_Table_NumberLength",
                table: "SequenceNumbers",
                columns: new[] { "Prefix", "TableName", "NumberLength" },
                unique: true);

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
                name: "IX_ShiftLogItems_ItemId",
                table: "ShiftLogItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogItems_ShiftLogId",
                table: "ShiftLogItems",
                column: "ShiftLogId");

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
                name: "IX_ShiftLogs_AssetId",
                table: "ShiftLogs",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_BoxId",
                table: "ShiftLogs",
                column: "BoxId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_CreatedByUserId",
                table: "ShiftLogs",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_ModifiedByUserId",
                table: "ShiftLogs",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLogs_OperationShiftId",
                table: "ShiftLogs",
                column: "OperationShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_BaseUnitId",
                table: "UnitOfMeasures",
                column: "BaseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_Code",
                table: "UnitOfMeasures",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_IsActive_UnitType",
                table: "UnitOfMeasures",
                columns: new[] { "IsActive", "UnitType" });

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_Symbol",
                table: "UnitOfMeasures",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_UnitType",
                table: "UnitOfMeasures",
                column: "UnitType");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_Subfolder",
                table: "UploadedFiles",
                column: "Subfolder");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_UploadedAt",
                table: "UploadedFiles",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_AccessTokenJti",
                table: "UserSessions",
                column: "AccessTokenJti");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_RefreshTokenJti",
                table: "UserSessions",
                column: "RefreshTokenJti");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId_RevokedAt",
                table: "UserSessions",
                columns: new[] { "UserId", "RevokedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetAdditionLines");

            migrationBuilder.DropTable(
                name: "AssetModelImages");

            migrationBuilder.DropTable(
                name: "AssetModelParameters");

            migrationBuilder.DropTable(
                name: "AssetParameters");

            migrationBuilder.DropTable(
                name: "AssetTypeParameters");

            migrationBuilder.DropTable(
                name: "IncidentReports");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "MaintenancePlanJobStepDefinitions");

            migrationBuilder.DropTable(
                name: "MaintenancePlanRequiredItems");

            migrationBuilder.DropTable(
                name: "OperationShiftAssetBoxes");

            migrationBuilder.DropTable(
                name: "OperationShiftAssets");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "ParameterBasedMaintenanceTriggers");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SequenceNumbers");

            migrationBuilder.DropTable(
                name: "ShiftLogCheckpoints");

            migrationBuilder.DropTable(
                name: "ShiftLogEvents");

            migrationBuilder.DropTable(
                name: "ShiftLogItems");

            migrationBuilder.DropTable(
                name: "ShiftLogParameterReadings");

            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "ParameterCatalogs");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "OperationShifts");

            migrationBuilder.DropTable(
                name: "MaintenancePlanDefinitions");

            migrationBuilder.DropTable(
                name: "ShiftLogs");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UnitOfMeasures");

            migrationBuilder.DropTable(
                name: "AssetAdditions");

            migrationBuilder.DropTable(
                name: "AssetModels");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "OrganizationUnits");

            migrationBuilder.DropTable(
                name: "AssetTypes");

            migrationBuilder.DropTable(
                name: "OrganizationUnitLevels");

            migrationBuilder.DropTable(
                name: "AssetCategories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
