using Domain.Enums;

namespace Application.CQRS.SeatReserve.DTOs
{
    public class SeatMapDto
    {
        public int FlightId { get; set; }
        public List<SeatClassGroupDto> ClassGroups { get; set; } = new();
    }

    public class SeatClassGroupDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public List<SeatRowDto> Rows { get; set; } = new();
    }

    public class SeatRowDto
    {
        public int RowIndex { get; set; }
        public List<SeatCellDto> Seats { get; set; } = new();
    }

    public class SeatCellDto
    {
        public int FlightSeatId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public int ColIndex { get; set; }
        public SeatStatus Status { get; set; }
        public int? LockedByPassengerId { get; set; }
    }
}
