using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TvTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalId",
                table: "TvShows",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalId",
                table: "Episodes",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "TvShows");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Episodes");
        }
    }
}
