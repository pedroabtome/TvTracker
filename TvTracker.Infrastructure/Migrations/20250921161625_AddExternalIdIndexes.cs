using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TvTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalIdIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Episodes_TvShowId",
                table: "Episodes");

            migrationBuilder.CreateIndex(
                name: "IX_TvShows_ExternalId",
                table: "TvShows",
                column: "ExternalId",
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_TvShowId_ExternalId",
                table: "Episodes",
                columns: new[] { "TvShowId", "ExternalId" },
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TvShows_ExternalId",
                table: "TvShows");

            migrationBuilder.DropIndex(
                name: "IX_Episodes_TvShowId_ExternalId",
                table: "Episodes");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_TvShowId",
                table: "Episodes",
                column: "TvShowId");
        }
    }
}
