using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Librarian.Sephirah.Migrations
{
    /// <inheritdoc />
    public partial class FixUserAppPackageTotalRunTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalRunTime",
                table: "AppPackages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalRunTime",
                table: "AppPackages",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
