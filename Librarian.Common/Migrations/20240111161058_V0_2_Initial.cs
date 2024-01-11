using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Common.Migrations
{
    /// <inheritdoc />
    public partial class V0_2_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Apps",
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
                    HeroImageUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ParentAppId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apps_Apps_ParentAppId",
                        column: x => x.ParentAppId,
                        principalTable: "Apps",
                        principalColumn: "Id");
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
                    GameSaveFileUsedCapacityBytes = table.Column<long>(type: "bigint", nullable: false),
                    GameSaveFileCapacityBytes = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppDetails",
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
                    AppId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDetails_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppPackages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    SourceAppId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AppId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPackages_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "Apps",
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
                name: "AppUser",
                columns: table => new
                {
                    AppsId = table.Column<long>(type: "bigint", nullable: false),
                    UsersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => new { x.AppsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AppUser_Apps_AppsId",
                        column: x => x.AppsId,
                        principalTable: "Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GameSaveFileRotations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EntityInternalId = table.Column<long>(type: "bigint", nullable: true),
                    ValidScope = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaveFileRotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSaveFileRotations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppPackagesBinaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    PublicUrl = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sha256 = table.Column<byte[]>(type: "binary(32)", fixedLength: true, maxLength: 32, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    AppPackageId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPackagesBinaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPackagesBinaries_AppPackages_AppPackageId",
                        column: x => x.AppPackageId,
                        principalTable: "AppPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GameSaveFiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsPinned = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FileMetadataId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AppPackageId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaveFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSaveFiles_AppPackages_AppPackageId",
                        column: x => x.AppPackageId,
                        principalTable: "AppPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSaveFiles_FileMetadatas_FileMetadataId",
                        column: x => x.FileMetadataId,
                        principalTable: "FileMetadatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSaveFiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserAppPackageRunTimes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AppPackageId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAppPackageRunTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAppPackageRunTimes_AppPackages_AppPackageId",
                        column: x => x.AppPackageId,
                        principalTable: "AppPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAppPackageRunTimes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserAppPackages",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AppPackageId = table.Column<long>(type: "bigint", nullable: false),
                    TotalRunTime = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAppPackages", x => new { x.UserId, x.AppPackageId });
                    table.ForeignKey(
                        name: "FK_UserAppPackages_AppPackages_AppPackageId",
                        column: x => x.AppPackageId,
                        principalTable: "AppPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAppPackages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserAppAppCategories",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AppId = table.Column<long>(type: "bigint", nullable: false),
                    AppCategoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAppAppCategories", x => new { x.UserId, x.AppId, x.AppCategoryId });
                    table.ForeignKey(
                        name: "FK_UserAppAppCategories_AppCategories_AppCategoryId",
                        column: x => x.AppCategoryId,
                        principalTable: "AppCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAppAppCategories_Apps_AppId",
                        column: x => x.AppId,
                        principalTable: "Apps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAppAppCategories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "IX_AppDetails_AppId",
                table: "AppDetails",
                column: "AppId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppPackages_AppId",
                table: "AppPackages",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPackages_CreatedAt",
                table: "AppPackages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppPackages_Source",
                table: "AppPackages",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_AppPackages_SourceAppId",
                table: "AppPackages",
                column: "SourceAppId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPackages_UpdatedAt",
                table: "AppPackages",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppPackagesBinaries_AppPackageId",
                table: "AppPackagesBinaries",
                column: "AppPackageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppPackagesBinaries_CreatedAt",
                table: "AppPackagesBinaries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppPackagesBinaries_UpdatedAt",
                table: "AppPackagesBinaries",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_CreatedAt",
                table: "Apps",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_Name",
                table: "Apps",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_ParentAppId",
                table: "Apps",
                column: "ParentAppId");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_Source",
                table: "Apps",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_SourceAppId",
                table: "Apps",
                column: "SourceAppId");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_Type",
                table: "Apps",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Apps_UpdatedAt",
                table: "Apps",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_UsersId",
                table: "AppUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadatas_CreatedAt",
                table: "FileMetadatas",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadatas_UpdatedAt",
                table: "FileMetadatas",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveFileRotations_EntityInternalId",
                table: "GameSaveFileRotations",
                column: "EntityInternalId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveFileRotations_UserId",
                table: "GameSaveFileRotations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveFiles_AppPackageId",
                table: "GameSaveFiles",
                column: "AppPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveFiles_CreatedAt",
                table: "GameSaveFiles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveFiles_FileMetadataId",
                table: "GameSaveFiles",
                column: "FileMetadataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveFiles_UpdatedAt",
                table: "GameSaveFiles",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveFiles_UserId",
                table: "GameSaveFiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAppAppCategories_AppCategoryId",
                table: "UserAppAppCategories",
                column: "AppCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAppAppCategories_AppId",
                table: "UserAppAppCategories",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAppPackageRunTimes_AppPackageId",
                table: "UserAppPackageRunTimes",
                column: "AppPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAppPackageRunTimes_UserId",
                table: "UserAppPackageRunTimes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAppPackages_AppPackageId",
                table: "UserAppPackages",
                column: "AppPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedAt",
                table: "Users",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDetails");

            migrationBuilder.DropTable(
                name: "AppPackagesBinaries");

            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropTable(
                name: "GameSaveFileRotations");

            migrationBuilder.DropTable(
                name: "GameSaveFiles");

            migrationBuilder.DropTable(
                name: "UserAppAppCategories");

            migrationBuilder.DropTable(
                name: "UserAppPackageRunTimes");

            migrationBuilder.DropTable(
                name: "UserAppPackages");

            migrationBuilder.DropTable(
                name: "FileMetadatas");

            migrationBuilder.DropTable(
                name: "AppCategories");

            migrationBuilder.DropTable(
                name: "AppPackages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Apps");
        }
    }
}
