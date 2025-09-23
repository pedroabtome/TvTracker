using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TvTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImdbRatingAndCasting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Episodes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ImdbRating",
                table: "Episodes",
                type: "numeric(3,1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImdbRating",
                table: "Episodes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Episodes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);
        }
    }
}
