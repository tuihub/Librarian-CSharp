using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbToProto0236_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAppAppCategories_AppCategory_AppCategoryId",
                table: "UserAppAppCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppCategory",
                table: "AppCategory");

            migrationBuilder.RenameTable(
                name: "AppCategory",
                newName: "AppCategories");

            migrationBuilder.RenameIndex(
                name: "IX_AppCategory_UpdatedAt",
                table: "AppCategories",
                newName: "IX_AppCategories_UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_AppCategory_CreatedAt",
                table: "AppCategories",
                newName: "IX_AppCategories_CreatedAt");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "AppCategories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppCategories",
                table: "AppCategories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppCategories_UserId",
                table: "AppCategories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCategories_Users_UserId",
                table: "AppCategories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAppAppCategories_AppCategories_AppCategoryId",
                table: "UserAppAppCategories",
                column: "AppCategoryId",
                principalTable: "AppCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCategories_Users_UserId",
                table: "AppCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAppAppCategories_AppCategories_AppCategoryId",
                table: "UserAppAppCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppCategories",
                table: "AppCategories");

            migrationBuilder.DropIndex(
                name: "IX_AppCategories_UserId",
                table: "AppCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppCategories");

            migrationBuilder.RenameTable(
                name: "AppCategories",
                newName: "AppCategory");

            migrationBuilder.RenameIndex(
                name: "IX_AppCategories_UpdatedAt",
                table: "AppCategory",
                newName: "IX_AppCategory_UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_AppCategories_CreatedAt",
                table: "AppCategory",
                newName: "IX_AppCategory_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppCategory",
                table: "AppCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAppAppCategories_AppCategory_AppCategoryId",
                table: "UserAppAppCategories",
                column: "AppCategoryId",
                principalTable: "AppCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
