using Application.Common;
using Application.CQRS.Dashboard.DTOs;
using MediatR;

namespace Application.CQRS.Dashboard.Queries.GetRevenue
{
    public class GetRevenueByYearQuery : IRequest<ApiResult<List<MonthlyRevenueDto>>>
    {
        public int Year { get; set; } = DateTime.UtcNow.Year;
    }
}