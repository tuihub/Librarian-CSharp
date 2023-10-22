using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    /// <inheritdoc />
    public partial class FixGameSaveFileRotationAndAppPackageRunTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "EntityInternalId",
                table: "GameSaveFileRotations",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "GameSaveFileRotations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "AppPackageRunTimes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveFileRotations_UserId",
                table: "GameSaveFileRotations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPackageRunTimes_UserId",
                table: "AppPackageRunTimes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPackageRunTimes_Users_UserId",
                table: "AppPackageRunTimes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSaveFileRotations_Users_UserId",
                table: "GameSaveFileRotations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPackageRunTimes_Users_UserId",
                table: "AppPackageRunTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSaveFileRotations_Users_UserId",
                table: "GameSaveFileRotations");

            migrationBuilder.DropIndex(
                name: "IX_GameSaveFileRotations_UserId",
                table: "GameSaveFileRotations");

            migrationBuilder.DropIndex(
                name: "IX_AppPackageRunTimes_UserId",
                table: "AppPackageRunTimes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GameSaveFileRotations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppPackageRunTimes");

            migrationBuilder.AlterColumn<long>(
                name: "EntityInternalId",
                table: "GameSaveFileRotations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
