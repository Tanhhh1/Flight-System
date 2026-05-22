namespace Application.CQRS.Statistics.DTOs
{
    public class StatisticDtos
    {
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TodayBookings { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveFlights { get; set; }
        public List<RevenueMonthDto> RevenueByMonth { get; set; } = new();
    }

    public class RevenueMonthDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int TotalBookings { get; set; }
    }
}
