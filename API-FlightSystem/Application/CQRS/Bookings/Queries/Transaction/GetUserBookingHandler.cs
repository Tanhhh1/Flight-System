using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Bookings.Queries.Transaction
{
    public class GetUserBookingHandler : IRequestHandler<GetUserBookingQuery, ApiResult<PageList<BookingDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public GetUserBookingHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ApiResult<PageList<BookingDto>>> Handle(GetUserBookingQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id is null)
                return ApiResult<PageList<BookingDto>>.Failure(["Bạn cần đăng nhập để xem danh sách booking"]);

            var query = _unitOfWork.BookingRepository
                .GetByCondition(b => b.UserId == _currentUser.Id)
                .Include(b => b.SeatClass)
                .Include(b => b.BookingDetails).ThenInclude(d => d.Passenger)
                .Include(b => b.BookingDetails).ThenInclude(d => d.Flight)
                .AsNoTracking()
                .OrderByDescending(b => b.BookingDate);

            var pagedList = await PageList<BookingDto>.ToPagedListAsync(
                query.ProjectToType<BookingDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<BookingDto>>.Success(pagedList);
        }
    }
}
