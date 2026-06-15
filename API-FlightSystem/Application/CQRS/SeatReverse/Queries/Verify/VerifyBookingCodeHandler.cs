using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.SeatReverse.DTOs;
using Application.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Application.CQRS.SeatReverse.Queries.Verify
{
    public class VerifyBookingHandler : IRequestHandler<VerifyBookingQuery, ApiResult<VerifyBookingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerifyBookingHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<VerifyBookingDto>> Handle(VerifyBookingQuery request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingCode == request.BookingCode)
                .Include(b => b.SeatClass)
                .Include(b => b.BookingDetails).ThenInclude(bd => bd.Passenger)
                .Include(b => b.BookingDetails).ThenInclude(bd => bd.Flight).ThenInclude(f => f.Route).ThenInclude(r => r.OriginAirport)
                .Include(b => b.BookingDetails).ThenInclude(bd => bd.Flight).ThenInclude(f => f.Route).ThenInclude(r => r.DestinationAirport)
                .Include(b => b.BookingDetails).ThenInclude(bd => bd.FlightSeat).ThenInclude(fs => fs != null ? fs.SeatTemplate : null)
                .FirstOrDefaultAsync(cancellationToken);

            if (booking == null)
                return ApiResult<VerifyBookingDto>.Failure("Booking code không hợp lệ.");

            var flightGroups = booking.BookingDetails
                .GroupBy(bd => bd.BookingFlightId)
                .Select(g => new VerifyBookingFlightDto
                {
                    FlightId = g.Key,
                    OriginCode = g.First().Flight.Route.OriginAirport.AirportCode,
                    DestinationCode = g.First().Flight.Route.DestinationAirport.AirportCode,
                    DepartureTime = g.First().Flight.DepartureTime,
                    Passengers = g.Select(bd => new BookingPassengerDto
                    {
                        BookingDetailId = bd.BookingDetailId,
                        PassengerId = bd.PassengerId,
                        FullName = bd.Passenger.FullName,
                        Gender = bd.Passenger.Gender,
                        FlightSeatId = bd.FlightSeatId,
                        SeatNumber = bd.FlightSeat?.SeatTemplate?.SeatNumber
                    }).ToList()
                }).ToList();

            var dto = new VerifyBookingDto
            {
                BookingId = booking.BookingId,
                BookingCode = booking.BookingCode,
                ClassId = booking.ClassId,
                ClassName = booking.SeatClass.ClassName,
                TripType = booking.TripType,
                Flights = flightGroups
            };

            return ApiResult<VerifyBookingDto>.Success(dto);
        }
    }
}
