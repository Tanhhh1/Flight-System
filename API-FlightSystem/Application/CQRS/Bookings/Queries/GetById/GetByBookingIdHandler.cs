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

        public async Task<ApiResult<BookingDto>> Handle(GetByBookingIdQuery request,CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingId == request.BookingId)
                .Include(b => b.User)
                .Include(b => b.SeatClass)
                .Include(b => b.BookingDetails).ThenInclude(d => d.Passenger)
                .Include(b => b.BookingDetails).ThenInclude(d => d.Flight)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (booking is null)
                return ApiResult<BookingDto>.Failure([$"Không tìm thấy booking với Id: {request.BookingId}"]);

            var dto = booking.Adapt<BookingDto>();
            dto.UserFullname = booking.User.Fullname;
            dto.ClassName = booking.SeatClass.ClassName;
            dto.TripType = ((Domain.Enums.TripType)booking.TripType) switch
            {
                Domain.Enums.TripType.OneWay => "Một chiều",
                Domain.Enums.TripType.RoundTrip => "Khứ hồi",
                Domain.Enums.TripType.MultiCity => "Nhiều điểm đến",
                _ => "Không xác định"
            };

            return ApiResult<BookingDto>.Success(dto);
        }
    }
}
