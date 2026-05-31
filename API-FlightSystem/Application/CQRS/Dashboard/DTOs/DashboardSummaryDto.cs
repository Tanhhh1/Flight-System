namespace Application.CQRS.Dashboard.DTOs
{
    public class DashboardSummaryDto
    {
        public int ActiveFlights { get; set; }
        public int TicketsSoldThisMonth { get; set; }
        public int NewMembersThisMonth { get; set; }
        public decimal RevenueThisMonth { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public int Month { get; set; }
        public decimal Revenue { get; set; }
    }
}