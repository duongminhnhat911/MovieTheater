using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieManagementWeb_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMovieEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrailerUrl",
                table: "Movies",
                newName: "TrailerLink");

            migrationBuilder.RenameColumn(
                name: "PosterUrl",
                table: "Movies",
                newName: "Image");

            migrationBuilder.AddColumn<string>(
                name: "CarouselImage",
                table: "Movies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cast",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Movies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Format",
                table: "Movies",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ProductionCompany",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RatingCode",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tagline",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarouselImage",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Cast",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "ProductionCompany",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "RatingCode",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Tagline",
                table: "Movies");

            migrationBuilder.RenameColumn(
                name: "TrailerLink",
                table: "Movies",
                newName: "TrailerUrl");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Movies",
                newName: "PosterUrl");
        }
    }
}
