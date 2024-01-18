using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Common.Migrations
{
    /// <inheritdoc />
    public partial class ProtoV0_3_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HeroImageUrl",
                table: "Apps",
                newName: "CoverImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImageUrl",
                table: "Apps",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TokenServerUrl",
                table: "AppPackagesBinaries",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AppPackagesBinariesChunks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Sequence = table.Column<long>(type: "bigint", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    PublicUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sha256 = table.Column<byte[]>(type: "longblob", nullable: true),
                    AppPackageBinaryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPackagesBinariesChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPackagesBinariesChunks_AppPackagesBinaries_AppPackageBina~",
                        column: x => x.AppPackageBinaryId,
                        principalTable: "AppPackagesBinaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AppPackagesBinariesChunks_AppPackageBinaryId",
                table: "AppPackagesBinariesChunks",
                column: "AppPackageBinaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPackagesBinariesChunks");

            migrationBuilder.DropColumn(
                name: "BackgroundImageUrl",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "TokenServerUrl",
                table: "AppPackagesBinaries");

            migrationBuilder.RenameColumn(
                name: "CoverImageUrl",
                table: "Apps",
                newName: "HeroImageUrl");
        }
    }
}
