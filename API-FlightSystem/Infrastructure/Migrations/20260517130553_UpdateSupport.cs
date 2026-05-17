using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NewFlightId",
                table: "SupportRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_NewFlightId",
                table: "SupportRequests",
                column: "NewFlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportRequests_Flights_NewFlightId",
                table: "SupportRequests",
                column: "NewFlightId",
                principalTable: "Flights",
                principalColumn: "FlightId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportRequests_Flights_NewFlightId",
                table: "SupportRequests");

            migrationBuilder.DropIndex(
                name: "IX_SupportRequests_NewFlightId",
                table: "SupportRequests");

            migrationBuilder.DropColumn(
                name: "NewFlightId",
                table: "SupportRequests");
        }
    }
}
