using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Common.Migrations.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class V0_2_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    DeviceName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    SystemType = table.Column<int>(type: "INTEGER", nullable: false),
                    SystemVersion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ClientName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ClientSourceCodeAddress = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    ClientVersion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureFlags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdStr = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Region = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    ConfigJsonSchema = table.Column<string>(type: "TEXT", maxLength: 65535, nullable: false),
                    RequireContext = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureFlags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedItemCollections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    CategoryStr = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedItemCollections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileMetadatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Sha256 = table.Column<byte[]>(type: "BLOB", fixedLength: true, maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetadatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Porters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    GlobalName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FeatureSummary = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    ContextJsonSchema = table.Column<string>(type: "TEXT", maxLength: 65535, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PorterContextId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Porters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreApps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    AppSources = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    IconImageId = table.Column<long>(type: "INTEGER", nullable: false),
                    BackgroundImageId = table.Column<long>(type: "INTEGER", nullable: false),
                    CoverImageId = table.Column<long>(type: "INTEGER", nullable: false),
                    Developer = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Publisher = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AltNames = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreApps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AppAppSaveFileCapacityCountDefault = table.Column<long>(type: "INTEGER", nullable: false),
                    AppAppSaveFileCapacitySizeBytesDefault = table.Column<long>(type: "INTEGER", nullable: false),
                    AppAppSaveFileCapacityStrategyDefault = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalAppSaveFileCount = table.Column<long>(type: "INTEGER", nullable: false),
                    TotalAppSaveFileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdStr = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Region = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ConfigJson = table.Column<string>(type: "TEXT", maxLength: 65535, nullable: false),
                    ContextId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FeatureFlagId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureRequests_FeatureFlags_FeatureFlagId",
                        column: x => x.FeatureFlagId,
                        principalTable: "FeatureFlags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PorterContexts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    ContextJson = table.Column<string>(type: "TEXT", maxLength: 65535, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PorterId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PorterContexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PorterContexts_Porters_PorterId",
                        column: x => x.PorterId,
                        principalTable: "Porters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PlatformAccountId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ProfileUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AvatarUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "AppCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "AppSaveFileCapacities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<int>(type: "INTEGER", nullable: false),
                    EntityInternalId = table.Column<long>(type: "INTEGER", nullable: false),
                    Count = table.Column<long>(type: "INTEGER", nullable: false),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Strategy = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "DeviceUser",
                columns: table => new
                {
                    DevicesId = table.Column<long>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<long>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Sentinels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 511, nullable: false),
                    AltUrls = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    GetTokenUrlPath = table.Column<string>(type: "TEXT", maxLength: 511, nullable: false),
                    DownloadFileUrlPath = table.Column<string>(type: "TEXT", maxLength: 511, nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", maxLength: 1023, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InternalId = table.Column<long>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    TokenJti = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<long>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "FeedConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PullInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    HideItems = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastPullTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastPullStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    LastPullMessage = table.Column<string>(type: "TEXT", maxLength: 65535, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SourceId = table.Column<long>(type: "INTEGER", nullable: false),
                    FeedItemCollectionId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedConfigs_FeatureRequests_SourceId",
                        column: x => x.SourceId,
                        principalTable: "FeatureRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedConfigs_FeedItemCollections_FeedItemCollectionId",
                        column: x => x.FeedItemCollectionId,
                        principalTable: "FeedItemCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Source = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceAppId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    SourceUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    IconImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    IconImageId = table.Column<long>(type: "INTEGER", nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    BackgroundImageId = table.Column<long>(type: "INTEGER", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CoverImageId = table.Column<long>(type: "INTEGER", nullable: false),
                    Developer = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Publisher = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AltNames = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: true),
                    AccountId = table.Column<long>(type: "INTEGER", nullable: true)
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
                        name: "FK_AppInfos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SentinelLibraries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LibraryId = table.Column<long>(type: "INTEGER", nullable: false),
                    DownloadBasePath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentinelId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentinelLibraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SentinelLibraries_Sentinels_SentinelId",
                        column: x => x.SentinelId,
                        principalTable: "Sentinels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedActions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FeedConfigId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedActions_FeedConfigs_FeedConfigId",
                        column: x => x.FeedConfigId,
                        principalTable: "FeedConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Apps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    RevisedVersion = table.Column<ulong>(type: "INTEGER", nullable: false),
                    RevisedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorDeviceId = table.Column<long>(type: "INTEGER", nullable: false),
                    AppSources = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    BoundStoreAppId = table.Column<long>(type: "INTEGER", nullable: false),
                    StopStoreManage = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4095, nullable: false),
                    IconImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    IconImageId = table.Column<long>(type: "INTEGER", nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    BackgroundImageId = table.Column<long>(type: "INTEGER", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CoverImageId = table.Column<long>(type: "INTEGER", nullable: false),
                    Developer = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Publisher = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AltNames = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalRunTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TotalAppSaveFileCount = table.Column<long>(type: "INTEGER", nullable: false),
                    TotalAppSaveFileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    AppInfoId = table.Column<long>(type: "INTEGER", nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "StoreAppBinaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    SentinelId = table.Column<long>(type: "INTEGER", nullable: false),
                    SentinelGeneratedId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StoreAppId = table.Column<long>(type: "INTEGER", nullable: false),
                    AppInfoId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreAppBinaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreAppBinaries_AppInfos_AppInfoId",
                        column: x => x.AppInfoId,
                        principalTable: "AppInfos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoreAppBinaries_StoreApps_StoreAppId",
                        column: x => x.StoreAppId,
                        principalTable: "StoreApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SentinelAppBinaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GeneratedId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    NeedToken = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Developer = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Publisher = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ChunksInfo = table.Column<string>(type: "TEXT", maxLength: 65535, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SentinelLibraryId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentinelAppBinaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SentinelAppBinaries_SentinelLibraries_SentinelLibraryId",
                        column: x => x.SentinelLibraryId,
                        principalTable: "SentinelLibraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppAppCategory",
                columns: table => new
                {
                    AppCategoriesId = table.Column<long>(type: "INTEGER", nullable: false),
                    AppsId = table.Column<long>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "AppRunTimes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AppId = table.Column<long>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRunTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRunTimes_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppRunTimes_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppSaveFiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPinned = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FileMetadataId = table.Column<long>(type: "INTEGER", nullable: false),
                    AppId = table.Column<long>(type: "INTEGER", nullable: false)
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
                });

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
                name: "IX_AppRunTimes_AppId",
                table: "AppRunTimes",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRunTimes_CreatedAt",
                table: "AppRunTimes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppRunTimes_DeviceId",
                table: "AppRunTimes",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRunTimes_StartDateTime_EndDateTime",
                table: "AppRunTimes",
                columns: new[] { "StartDateTime", "EndDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AppRunTimes_UpdatedAt",
                table: "AppRunTimes",
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
                name: "IX_FeatureFlags_CreatedAt",
                table: "FeatureFlags",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureFlags_IdStr",
                table: "FeatureFlags",
                column: "IdStr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureFlags_UpdatedAt",
                table: "FeatureFlags",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureRequests_CreatedAt",
                table: "FeatureRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureRequests_FeatureFlagId",
                table: "FeatureRequests",
                column: "FeatureFlagId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureRequests_UpdatedAt",
                table: "FeatureRequests",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeedActions_CreatedAt",
                table: "FeedActions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeedActions_FeedConfigId",
                table: "FeedActions",
                column: "FeedConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedActions_UpdatedAt",
                table: "FeedActions",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeedConfigs_CreatedAt",
                table: "FeedConfigs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeedConfigs_FeedItemCollectionId",
                table: "FeedConfigs",
                column: "FeedItemCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedConfigs_SourceId",
                table: "FeedConfigs",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedConfigs_UpdatedAt",
                table: "FeedConfigs",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeedItemCollections_CategoryStr",
                table: "FeedItemCollections",
                column: "CategoryStr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedItemCollections_CreatedAt",
                table: "FeedItemCollections",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FeedItemCollections_UpdatedAt",
                table: "FeedItemCollections",
                column: "UpdatedAt");

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
                name: "IX_PorterContexts_CreatedAt",
                table: "PorterContexts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PorterContexts_PorterId",
                table: "PorterContexts",
                column: "PorterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PorterContexts_UpdatedAt",
                table: "PorterContexts",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Porters_CreatedAt",
                table: "Porters",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Porters_GlobalName",
                table: "Porters",
                column: "GlobalName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Porters_UpdatedAt",
                table: "Porters",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelAppBinaries_CreatedAt",
                table: "SentinelAppBinaries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelAppBinaries_SentinelLibraryId_GeneratedId",
                table: "SentinelAppBinaries",
                columns: new[] { "SentinelLibraryId", "GeneratedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SentinelAppBinaries_UpdatedAt",
                table: "SentinelAppBinaries",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelLibraries_CreatedAt",
                table: "SentinelLibraries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelLibraries_SentinelId",
                table: "SentinelLibraries",
                column: "SentinelId");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelLibraries_UpdatedAt",
                table: "SentinelLibraries",
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
                name: "IX_StoreAppBinaries_AppInfoId",
                table: "StoreAppBinaries",
                column: "AppInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreAppBinaries_CreatedAt",
                table: "StoreAppBinaries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StoreAppBinaries_SentinelGeneratedId",
                table: "StoreAppBinaries",
                column: "SentinelGeneratedId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreAppBinaries_StoreAppId",
                table: "StoreAppBinaries",
                column: "StoreAppId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreAppBinaries_UpdatedAt",
                table: "StoreAppBinaries",
                column: "UpdatedAt");

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
                name: "AppRunTimes");

            migrationBuilder.DropTable(
                name: "AppSaveFileCapacities");

            migrationBuilder.DropTable(
                name: "AppSaveFiles");

            migrationBuilder.DropTable(
                name: "DeviceUser");

            migrationBuilder.DropTable(
                name: "FeedActions");

            migrationBuilder.DropTable(
                name: "PorterContexts");

            migrationBuilder.DropTable(
                name: "SentinelAppBinaries");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "StoreAppBinaries");

            migrationBuilder.DropTable(
                name: "AppCategories");

            migrationBuilder.DropTable(
                name: "Apps");

            migrationBuilder.DropTable(
                name: "FileMetadatas");

            migrationBuilder.DropTable(
                name: "FeedConfigs");

            migrationBuilder.DropTable(
                name: "Porters");

            migrationBuilder.DropTable(
                name: "SentinelLibraries");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "StoreApps");

            migrationBuilder.DropTable(
                name: "AppInfos");

            migrationBuilder.DropTable(
                name: "FeatureRequests");

            migrationBuilder.DropTable(
                name: "FeedItemCollections");

            migrationBuilder.DropTable(
                name: "Sentinels");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "FeatureFlags");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
