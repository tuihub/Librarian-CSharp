using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbToProto0236 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Apps",
                newName: "IconImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "HeroImageUrl",
                table: "Apps",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCategory", x => x.Id);
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
                        name: "FK_UserAppAppCategories_AppCategory_AppCategoryId",
                        column: x => x.AppCategoryId,
                        principalTable: "AppCategory",
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
                name: "IX_AppCategory_CreatedAt",
                table: "AppCategory",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppCategory_UpdatedAt",
                table: "AppCategory",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserAppAppCategories_AppCategoryId",
                table: "UserAppAppCategories",
                column: "AppCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAppAppCategories_AppId",
                table: "UserAppAppCategories",
                column: "AppId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAppAppCategories");

            migrationBuilder.DropTable(
                name: "AppCategory");

            migrationBuilder.DropColumn(
                name: "HeroImageUrl",
                table: "Apps");

            migrationBuilder.RenameColumn(
                name: "IconImageUrl",
                table: "Apps",
                newName: "ImageUrl");
        }
    }
}
