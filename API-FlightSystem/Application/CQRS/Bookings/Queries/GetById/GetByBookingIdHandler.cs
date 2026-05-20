using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Bookings.Queries.GetById
{
    public class GetByBookingIdHandler : IRequestHandler<GetByBookingIdQuery, ApiResult<BookingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByBookingIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<BookingDto>> Handle(GetByBookingIdQuery request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingId == request.BookingId)
                .AsNoTracking()
                .ProjectToType<BookingDto>()
                .FirstOrDefaultAsync(cancellationToken);

            if (booking is null)
                return ApiResult<BookingDto>.Failure(["Không tìm thấy giao dịch"]);

            return ApiResult<BookingDto>.Success(booking);
        }
    }
}
