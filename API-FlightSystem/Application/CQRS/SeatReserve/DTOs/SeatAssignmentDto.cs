namespace Application.CQRS.SeatReserve.DTOs
{
    public class SeatAssignmentDto
    {
        public int PassengerId { get; set; }
        public int FlightSeatId { get; set; }
    }
}
