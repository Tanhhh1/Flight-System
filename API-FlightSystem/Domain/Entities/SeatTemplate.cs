using Domain.Common;

namespace Domain.Entities
{
    public class SeatTemplate : BaseEntity
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public SeatClass SeatClass { get; set; } = null!;
        public ICollection<FlightSeat> FlightSeats { get; set; } = new List<FlightSeat>();
    }
}
