using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SeatReverse.Queries.GetBookingPassengers
{
    public class GetBookingPassengersHandler : IRequestHandler<GetBookingPassengersQuery, ApiResult<BookingPassengersDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public GetBookingPassengersHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<ApiResult<BookingPassengersDto>> Handle(GetBookingPassengersQuery request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingCode == request.BookingCode && b.UserId == _currentUser.Id)
                .Include(b => b.BookingDetails).ThenInclude(bd => bd.Passenger)
                .Include(b => b.BookingDetails).ThenInclude(bd => bd.FlightSeat).ThenInclude(fs => fs!.SeatTemplate)
                .FirstOrDefaultAsync(cancellationToken);

            if (booking == null)
                return ApiResult<BookingPassengersDto>.Failure("Không tìm thấy đơn đặt vé");

            var result = new BookingPassengersDto
            {
                BookingId = booking.BookingId,
                BookingCode = booking.BookingCode,
                FlightId = booking.BookingDetails.First().FlightId,
                ClassId = booking.ClassId,
                ClassName = booking.SeatClass.ClassName,
                Passengers = _mapper.Map<List<PassengerSeatDto>>(booking.BookingDetails)
            };

            return ApiResult<BookingPassengersDto>.Success(result);
        }
    }
}
