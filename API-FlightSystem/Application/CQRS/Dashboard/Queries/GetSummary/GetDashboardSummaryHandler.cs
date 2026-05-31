using Application.Common;
using Application.CQRS.Dashboard.DTOs;
using Application.CQRS.Dashboard.Queries.GetSummary;
using Application.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Dashboard.Queries.GetSummary
{
    public class GetDashboardSummaryHandler : IRequestHandler<GetDashboardSummaryQuery, ApiResult<DashboardSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardSummaryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<DashboardSummaryDto>> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var activeFlights = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.Status == Domain.Enums.FlightStatus.Active)
                .AsNoTracking()
                .CountAsync(cancellationToken);

            var ticketsSoldThisMonth = await _unitOfWork.BookingRepository
                .GetByCondition(b =>
                    b.BookingDate >= startOfMonth &&
                    b.Status == Domain.Enums.BookingStatus.Confirmed)
                .AsNoTracking()
                .CountAsync(cancellationToken);

            var newMembersThisMonth = await _unitOfWork.BookingRepository
                .GetByCondition()
                .AsNoTracking()
                .Select(b => b.User)
                .Where(u => u.CreatedAt >= startOfMonth)
                .Distinct()
                .CountAsync(cancellationToken);

            var revenueThisMonth = await _unitOfWork.BookingRepository
                .GetByCondition(b =>
                    b.BookingDate >= startOfMonth &&
                    b.Status == Domain.Enums.BookingStatus.Confirmed)
                .AsNoTracking()
                .SumAsync(b => b.TotalPrice, cancellationToken);

            var result = new DashboardSummaryDto
            {
                ActiveFlights = activeFlights,
                TicketsSoldThisMonth = ticketsSoldThisMonth,
                NewMembersThisMonth = newMembersThisMonth,
                RevenueThisMonth = revenueThisMonth,
            };

            return ApiResult<DashboardSummaryDto>.Success(result);
        }
    }
}