using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieManagementWeb_API.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedEditedByToMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EditedByUsername",
                table: "Movies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "EditedByUsername",
                table: "Movies");
        }
    }
}
