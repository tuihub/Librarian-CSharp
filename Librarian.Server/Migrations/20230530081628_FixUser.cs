using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    /// <inheritdoc />
    public partial class FixUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GameSaveFileUsedCapacityBytes",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameSaveFileUsedCapacityBytes",
                table: "Users");
        }
    }
}
