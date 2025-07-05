using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingManagement.Migrations
{
    /// <inheritdoc />
    public partial class themcotTiTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "Showtimes");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ToTime",
                table: "Showtimes",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ToTime",
                table: "Showtimes");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ToDate",
                table: "Showtimes",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
