using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Flights_FlightId",
                table: "BookingDetails");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "FlightId",
                table: "BookingDetails",
                newName: "BookingFlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Flights_BookingFlightId",
                table: "BookingDetails",
                column: "BookingFlightId",
                principalTable: "Flights",
                principalColumn: "FlightId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Flights_BookingFlightId",
                table: "BookingDetails");

            migrationBuilder.RenameColumn(
                name: "BookingFlightId",
                table: "BookingDetails",
                newName: "FlightId");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Services",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Flights_FlightId",
                table: "BookingDetails",
                column: "FlightId",
                principalTable: "Flights",
                principalColumn: "FlightId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
