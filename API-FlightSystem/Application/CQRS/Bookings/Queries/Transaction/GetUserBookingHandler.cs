using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Bookings.Queries.Transaction
{
    public class GetUserBookingHandler : IRequestHandler<GetUserBookingQuery, ApiResult<PageList<BookingListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public GetUserBookingHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ApiResult<PageList<BookingListDto>>> Handle(GetUserBookingQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id is null)
                return ApiResult<PageList<BookingListDto>>.Failure("Bạn cần đăng nhập để xem danh sách giao dịch");

            var query = _unitOfWork.BookingRepository
                .GetByCondition(b => b.UserId == _currentUser.Id)
                .OrderByDescending(b => b.BookingDate)
                .AsNoTracking();

            var pagedList = await PageList<BookingListDto>.ToPagedListAsync(
                query.ProjectToType<BookingListDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<BookingListDto>>.Success(pagedList);
        }
    }
}
