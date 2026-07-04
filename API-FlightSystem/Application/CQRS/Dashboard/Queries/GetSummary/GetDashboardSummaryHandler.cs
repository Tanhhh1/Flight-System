using Application.Common;
using Application.CQRS.Dashboard.DTOs;
using Domain.Identity;
using Application.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Dashboard.Queries.GetSummary
{
    public class GetDashboardSummaryHandler : IRequestHandler<GetDashboardSummaryQuery, ApiResult<DashboardSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager; 

        public GetDashboardSummaryHandler(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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

            var totalAccount = await _userManager.Users
                .AsNoTracking()
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
                TotalAccount = totalAccount,
                RevenueThisMonth = revenueThisMonth,
            };

            return ApiResult<DashboardSummaryDto>.Success(result);
        }
    }
}