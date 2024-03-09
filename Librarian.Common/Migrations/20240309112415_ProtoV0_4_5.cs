using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Common.Migrations
{
    /// <inheritdoc />
    public partial class ProtoV0_4_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    DeviceName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SystemType = table.Column<int>(type: "int", nullable: false),
                    SystemVersion = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientName = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientSourceCodeAddress = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientVersion = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FileMetadatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Sha256 = table.Column<byte[]>(type: "binary(32)", fixedLength: true, maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetadatas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AppAppSaveFileCapacityCountDefault = table.Column<long>(type: "bigint", nullable: true),
                    AppAppSaveFileCapacitySizeBytesDefault = table.Column<long>(type: "bigint", nullable: true),
                    AppAppSaveFileCapacityStrategyDefault = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TotalAppSaveFileCount = table.Column<long>(type: "bigint", nullable: false),
                    TotalAppSaveFileSizeBytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Platform = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlatformAccountId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProfileUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AvatarUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCategories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppSaveFileCapacities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityInternalId = table.Column<long>(type: "bigint", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    Strategy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSaveFileCapacities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSaveFileCapacities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeviceUser",
                columns: table => new
                {
                    DevicesId = table.Column<long>(type: "bigint", nullable: false),
                    UsersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceUser", x => new { x.DevicesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_DeviceUser_Devices_DevicesId",
                        column: x => x.DevicesId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sentinels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Token = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentinels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sentinels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InternalId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TokenJti = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Source = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceAppId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ShortDescription = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IconImageUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BackgroundImageUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CoverImageUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ParentAppInfoId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    AccountId = table.Column<long>(type: "bigint", nullable: true),
                    IsInternal = table.Column<bool>(type: "tinyint(1)", nullable: false, computedColumnSql: "Source = 'internal'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppInfos_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppInfos_AppInfos_ParentAppInfoId",
                        column: x => x.ParentAppInfoId,
                        principalTable: "AppInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppInfos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppBinaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    PublicUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sha256 = table.Column<byte[]>(type: "binary(32)", fixedLength: true, maxLength: 32, nullable: true),
                    TokenServerUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AppInfoId = table.Column<long>(type: "bigint", nullable: false),
                    SentinelId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBinaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBinaries_AppInfos_AppInfoId",
                        column: x => x.AppInfoId,
                        principalTable: "AppInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppBinaries_Sentinels_SentinelId",
                        column: x => x.SentinelId,
                        principalTable: "Sentinels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppCategoryAppInfo",
                columns: table => new
                {
                    AppCategoriesId = table.Column<long>(type: "bigint", nullable: false),
                    AppInfosId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCategoryAppInfo", x => new { x.AppCategoriesId, x.AppInfosId });
                    table.ForeignKey(
                        name: "FK_AppCategoryAppInfo_AppCategories_AppCategoriesId",
                        column: x => x.AppCategoriesId,
                        principalTable: "AppCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppCategoryAppInfo_AppInfos_AppInfosId",
                        column: x => x.AppInfosId,
                        principalTable: "AppInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppInfoDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReleaseDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Developer = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Publisher = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Version = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrls = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AppId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInfoDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppInfoDetails_AppInfos_AppId",
                        column: x => x.AppId,
                        principalTable: "AppInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Apps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AppInfoId = table.Column<long>(type: "bigint", nullable: true),
                    TotalRunTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    TotalAppSaveFileCount = table.Column<long>(type: "bigint", nullable: false),
                    TotalAppSaveFileSizeBytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apps_AppInfos_AppInfoId",
                        column: x => x.AppInfoId,
                        principalTable: "AppInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apps_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppBinaryChunks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Sequence = table.Column<long>(type: "bigint", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    PublicUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sha256 = table.Column<byte[]>(type: "binary(32)", fixedLength: true, maxLength: 32, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AppBinaryId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBinaryChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBinaryChunks_AppBinaries_AppBinaryId",
                        column: x => x.AppBinaryId,
                        principalTable: "AppBinaries",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppAppCategory",
                columns: table => new
                {
                    AppCategoriesId = table.Column<long>(type: "bigint", nullable: false),
                    AppsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAppCategory", x => new { x.AppCategoriesId, x.AppsId });
                    table.ForeignKey(
                        name: "FK_AppAppCategory_AppCategories_AppCategoriesId",
                        column: x => x.AppCategoriesId,
                        principalTable: "AppCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppAppCategory_Apps_AppsId",
                        column: x => x.AppsId,
                        principalTable: "Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppInsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AppId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false),
                    TotalRunTime = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppInsts_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppInsts_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppSaveFiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsPinned = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FileMetadataId = table.Column<long>(type: "bigint", nullable: false),
                    AppId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSaveFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSaveFiles_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppSaveFiles_FileMetadatas_FileMetadataId",
                        column: x => x.FileMetadataId,
                        principalTable: "FileMetadatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppInstRunTimes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AppInstId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInstRunTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppInstRunTimes_AppInsts_AppInstId",
                        column: x => x.AppInstId,
                        principalTable: "AppInsts",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CreatedAt",
                table: "Accounts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Platform_PlatformAccountId",
                table: "Accounts",
                columns: new[] { "Platform", "PlatformAccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UpdatedAt",
                table: "Accounts",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAppCategory_AppsId",
                table: "AppAppCategory",
                column: "AppsId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaries_AppInfoId",
                table: "AppBinaries",
                column: "AppInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaries_CreatedAt",
                table: "AppBinaries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaries_SentinelId",
                table: "AppBinaries",
                column: "SentinelId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaries_TokenServerUrl",
                table: "AppBinaries",
                column: "TokenServerUrl");

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaries_UpdatedAt",
                table: "AppBinaries",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaryChunks_AppBinaryId",
                table: "AppBinaryChunks",
                column: "AppBinaryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaryChunks_CreatedAt",
                table: "AppBinaryChunks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaryChunks_UpdatedAt",
                table: "AppBinaryChunks",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppCategories_CreatedAt",
                table: "AppCategories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppCategories_UpdatedAt",
                table: "AppCategories",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppCategories_UserId",
                table: "AppCategories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCategoryAppInfo_AppInfosId",
                table: "AppCategoryAppInfo",
                column: "AppInfosId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfoDetails_AppId",
                table: "AppInfoDetails",
                column: "AppId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppInfoDetails_CreatedAt",
                table: "AppInfoDetails",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfoDetails_UpdatedAt",
                table: "AppInfoDetails",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfos_AccountId",
                table: "AppInfos",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfos_CreatedAt",
                table: "AppInfos",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfos_Name",
                table: "AppInfos",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfos_ParentAppInfoId",
                table: "AppInfos",
                column: "ParentAppInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfos_Source_SourceAppId",
                table: "AppInfos",
                columns: new[] { "Source", "SourceAppId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppInfos_Type",
                table: "AppInfos",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfos_UpdatedAt",
                table: "AppInfos",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppInfos_UserId",
                table: "AppInfos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInstRunTimes_AppInstId",
                table: "AppInstRunTimes",
                column: "AppInstId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInstRunTimes_CreatedAt",
                table: "AppInstRunTimes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppInstRunTimes_UpdatedAt",
                table: "AppInstRunTimes",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppInsts_AppId",
                table: "AppInsts",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInsts_CreatedAt",
                table: "AppInsts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppInsts_DeviceId",
                table: "AppInsts",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInsts_UpdatedAt",
                table: "AppInsts",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_AppInfoId",
                table: "Apps",
                column: "AppInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_CreatedAt",
                table: "Apps",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_IsPublic",
                table: "Apps",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_Name",
                table: "Apps",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_UpdatedAt",
                table: "Apps",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_UserId",
                table: "Apps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFileCapacities_CreatedAt",
                table: "AppSaveFileCapacities",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFileCapacities_EntityType_EntityInternalId",
                table: "AppSaveFileCapacities",
                columns: new[] { "EntityType", "EntityInternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFileCapacities_UpdatedAt",
                table: "AppSaveFileCapacities",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFileCapacities_UserId",
                table: "AppSaveFileCapacities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFiles_AppId",
                table: "AppSaveFiles",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFiles_CreatedAt",
                table: "AppSaveFiles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFiles_FileMetadataId",
                table: "AppSaveFiles",
                column: "FileMetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFiles_IsPinned",
                table: "AppSaveFiles",
                column: "IsPinned");

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFiles_Status",
                table: "AppSaveFiles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppSaveFiles_UpdatedAt",
                table: "AppSaveFiles",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_CreatedAt",
                table: "Devices",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceName",
                table: "Devices",
                column: "DeviceName");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SystemType",
                table: "Devices",
                column: "SystemType");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UpdatedAt",
                table: "Devices",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceUser_UsersId",
                table: "DeviceUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadatas_CreatedAt",
                table: "FileMetadatas",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadatas_Name",
                table: "FileMetadatas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadatas_SizeBytes",
                table: "FileMetadatas",
                column: "SizeBytes");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadatas_Type",
                table: "FileMetadatas",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadatas_UpdatedAt",
                table: "FileMetadatas",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sentinels_CreatedAt",
                table: "Sentinels",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sentinels_UpdatedAt",
                table: "Sentinels",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sentinels_UserId",
                table: "Sentinels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CreatedAt",
                table: "Sessions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_DeviceId",
                table: "Sessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ExpiredAt",
                table: "Sessions",
                column: "ExpiredAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_InternalId",
                table: "Sessions",
                column: "InternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Status",
                table: "Sessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TokenJti",
                table: "Sessions",
                column: "TokenJti");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UpdatedAt",
                table: "Sessions",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Status",
                table: "Users",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Type",
                table: "Users",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedAt",
                table: "Users",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAppCategory");

            migrationBuilder.DropTable(
                name: "AppBinaryChunks");

            migrationBuilder.DropTable(
                name: "AppCategoryAppInfo");

            migrationBuilder.DropTable(
                name: "AppInfoDetails");

            migrationBuilder.DropTable(
                name: "AppInstRunTimes");

            migrationBuilder.DropTable(
                name: "AppSaveFileCapacities");

            migrationBuilder.DropTable(
                name: "AppSaveFiles");

            migrationBuilder.DropTable(
                name: "DeviceUser");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "AppBinaries");

            migrationBuilder.DropTable(
                name: "AppCategories");

            migrationBuilder.DropTable(
                name: "AppInsts");

            migrationBuilder.DropTable(
                name: "FileMetadatas");

            migrationBuilder.DropTable(
                name: "Sentinels");

            migrationBuilder.DropTable(
                name: "Apps");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "AppInfos");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
