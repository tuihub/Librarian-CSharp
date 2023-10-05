using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    /// <inheritdoc />
    public partial class AddSelfOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentAppId",
                table: "Apps",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Apps_ParentAppId",
                table: "Apps",
                column: "ParentAppId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apps_Apps_ParentAppId",
                table: "Apps",
                column: "ParentAppId",
                principalTable: "Apps",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apps_Apps_ParentAppId",
                table: "Apps");

            migrationBuilder.DropIndex(
                name: "IX_Apps_ParentAppId",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "ParentAppId",
                table: "Apps");
        }
    }
}
