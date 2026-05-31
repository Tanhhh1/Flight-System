using Application.Common;
using Application.CQRS.Dashboard.DTOs;
using Application.CQRS.Dashboard.Queries.GetRevenue;
using Application.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Dashboard.Queries.GetRevenue
{
    public class GetRevenueByYearHandler : IRequestHandler<GetRevenueByYearQuery, ApiResult<List<MonthlyRevenueDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRevenueByYearHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<List<MonthlyRevenueDto>>> Handle(GetRevenueByYearQuery request, CancellationToken cancellationToken)
        {
            var revenueByMonth = await _unitOfWork.BookingRepository
                .GetByCondition(b =>
                    b.BookingDate.Year == request.Year &&
                    b.Status == Domain.Enums.BookingStatus.Confirmed)
                .AsNoTracking()
                .GroupBy(b => b.BookingDate.Month)
                .Select(g => new MonthlyRevenueDto
                {
                    Month = g.Key,
                    Revenue = g.Sum(b => b.TotalPrice),
                })
                .ToListAsync(cancellationToken);

            var result = Enumerable.Range(1, 12)
                .Select(m => revenueByMonth.FirstOrDefault(r => r.Month == m)
                    ?? new MonthlyRevenueDto { Month = m, Revenue = 0 })
                .ToList();

            return ApiResult<List<MonthlyRevenueDto>>.Success(result);
        }
    }
}