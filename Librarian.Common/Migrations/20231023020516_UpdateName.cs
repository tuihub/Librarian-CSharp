using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    /// <inheritdoc />
    public partial class UpdateName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPackageRunTimes_AppPackages_AppPackageId",
                table: "AppPackageRunTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPackageRunTimes_Users_UserId",
                table: "AppPackageRunTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppPackageRunTimes",
                table: "AppPackageRunTimes");

            migrationBuilder.RenameTable(
                name: "AppPackageRunTimes",
                newName: "UserAppPackageRunTimes");

            migrationBuilder.RenameIndex(
                name: "IX_AppPackageRunTimes_UserId",
                table: "UserAppPackageRunTimes",
                newName: "IX_UserAppPackageRunTimes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AppPackageRunTimes_AppPackageId",
                table: "UserAppPackageRunTimes",
                newName: "IX_UserAppPackageRunTimes_AppPackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAppPackageRunTimes",
                table: "UserAppPackageRunTimes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAppPackageRunTimes_AppPackages_AppPackageId",
                table: "UserAppPackageRunTimes",
                column: "AppPackageId",
                principalTable: "AppPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAppPackageRunTimes_Users_UserId",
                table: "UserAppPackageRunTimes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAppPackageRunTimes_AppPackages_AppPackageId",
                table: "UserAppPackageRunTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAppPackageRunTimes_Users_UserId",
                table: "UserAppPackageRunTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAppPackageRunTimes",
                table: "UserAppPackageRunTimes");

            migrationBuilder.RenameTable(
                name: "UserAppPackageRunTimes",
                newName: "AppPackageRunTimes");

            migrationBuilder.RenameIndex(
                name: "IX_UserAppPackageRunTimes_UserId",
                table: "AppPackageRunTimes",
                newName: "IX_AppPackageRunTimes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAppPackageRunTimes_AppPackageId",
                table: "AppPackageRunTimes",
                newName: "IX_AppPackageRunTimes_AppPackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppPackageRunTimes",
                table: "AppPackageRunTimes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPackageRunTimes_AppPackages_AppPackageId",
                table: "AppPackageRunTimes",
                column: "AppPackageId",
                principalTable: "AppPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppPackageRunTimes_Users_UserId",
                table: "AppPackageRunTimes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
