using Domain.Common;

namespace Domain.Entities
{
    public class SeatClass : BaseEntity
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public ICollection<FlightSeatPrice> FlightSeatPrices { get; set; } = new List<FlightSeatPrice>();   
        public ICollection<SeatTemplate> SeatTemplates { get; set; } = new List<SeatTemplate>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
