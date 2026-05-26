using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Bookings.Queries.GetById
{
    public class GetByBookingIdHandler : IRequestHandler<GetByBookingIdQuery, ApiResult<BookingByIdDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByBookingIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<BookingByIdDto>> Handle(GetByBookingIdQuery request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingId == request.BookingId)
                .AsNoTracking()
                .ProjectToType<BookingByIdDto>()
                .FirstOrDefaultAsync(cancellationToken);

            if (booking is null)
                return ApiResult<BookingByIdDto>.Failure("Không tìm thấy giao dịch");

            return ApiResult<BookingByIdDto>.Success(booking);
        }
    }
}
