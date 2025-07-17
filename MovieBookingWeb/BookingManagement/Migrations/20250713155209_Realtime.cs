using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingManagement.Migrations
{
    /// <inheritdoc />
    public partial class Realtime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeldByUserId",
                table: "SeatShowtimes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HeldUntil",
                table: "SeatShowtimes",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeldByUserId",
                table: "SeatShowtimes");

            migrationBuilder.DropColumn(
                name: "HeldUntil",
                table: "SeatShowtimes");
        }
    }
}
