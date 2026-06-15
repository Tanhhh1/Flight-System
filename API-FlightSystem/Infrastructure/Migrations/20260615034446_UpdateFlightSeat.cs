using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlightSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlightSeat_Unique",
                table: "FlightSeats");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "FlightSeats");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "FlightSeats",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "FlightSeats");

            migrationBuilder.AddColumn<Guid>(
                name: "ConcurrencyStamp",
                table: "FlightSeats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FlightSeat_Unique",
                table: "FlightSeats",
                columns: new[] { "FlightId", "SeatId" },
                unique: true);
        }
    }
}
