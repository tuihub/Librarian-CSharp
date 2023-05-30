using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    /// <inheritdoc />
    public partial class AddRotationCapacityUpdateSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Size",
                table: "FileMetadatas",
                newName: "SizeBytes");

            migrationBuilder.RenameColumn(
                name: "SizeByte",
                table: "AppPackagesBinaries",
                newName: "SizeBytes");

            migrationBuilder.AddColumn<long>(
                name: "GameSaveFileCapacityBytes",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "GameSaveFiles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "GameSaveFileRotations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EntityInternalId = table.Column<long>(type: "bigint", nullable: false),
                    VaildScope = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaveFileRotations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaveFileRotations_EntityInternalId",
                table: "GameSaveFileRotations",
                column: "EntityInternalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSaveFileRotations");

            migrationBuilder.DropColumn(
                name: "GameSaveFileCapacityBytes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "GameSaveFiles");

            migrationBuilder.RenameColumn(
                name: "SizeBytes",
                table: "FileMetadatas",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "SizeBytes",
                table: "AppPackagesBinaries",
                newName: "SizeByte");
        }
    }
}
