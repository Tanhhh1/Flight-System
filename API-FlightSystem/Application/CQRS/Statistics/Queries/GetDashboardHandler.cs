using Application.Common;
using Application.CQRS.Statistics.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.CQRS.Statistics.Queries
{
    public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, ApiResult<StatisticDtos>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<StatisticDtos>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);

            var totalRevenue = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.Status == BookingStatus.Confirmed)
                .SumAsync(b => b.TotalPrice, cancellationToken);

            var totalBookings = await _unitOfWork.BookingRepository
                .GetByCondition()
                .CountAsync(cancellationToken);

            var todayBookings = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingDate.Date == today)
                .CountAsync(cancellationToken);

            var totalUsers = await _unitOfWork.BookingRepository
                .GetByCondition()
                .Select(b => b.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            var activeFlights = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.Status == FlightStatus.Active)
                .CountAsync(cancellationToken);

            var revenueByMonthRaw = await _unitOfWork.BookingRepository
                .GetByCondition(b =>
                    b.Status == BookingStatus.Confirmed &&
                    b.BookingDate >= twelveMonthsAgo)
                .GroupBy(b => new { b.BookingDate.Year, b.BookingDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(b => b.TotalPrice),
                    TotalBookings = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(cancellationToken);

            var revenueByMonth = revenueByMonthRaw
                .Select(x => new RevenueMonthDto
                {
                    Year = x.Year,
                    Month = x.Month,
                    Revenue = x.Revenue,
                    TotalBookings = x.TotalBookings
                }).ToList();

            var result = new StatisticDtos
            {
                TotalRevenue = totalRevenue,
                TotalBookings = totalBookings,
                TodayBookings = todayBookings,
                TotalUsers = totalUsers,
                ActiveFlights = activeFlights,
                RevenueByMonth = revenueByMonth
            };

            return ApiResult<StatisticDtos>.Success(result);
        }
    }
}
