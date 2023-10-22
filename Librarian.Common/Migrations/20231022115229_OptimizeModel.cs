using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_UserAppPackages_AppPackageId",
                table: "UserAppPackages",
                column: "AppPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAppPackages");
        }
    }
}
