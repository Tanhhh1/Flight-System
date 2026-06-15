using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Bookings.Commands.Booking
{
    public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, ApiResult<BookingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public CreateBookingHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ApiResult<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
                return ApiResult<BookingDto>.Failure("Bạn cần đăng nhập để đặt vé");

            var flightIds = request.FlightIds.Distinct().ToList();

            var existingFlightIds = await _unitOfWork.FlightRepository
                .GetByCondition(f => flightIds.Contains(f.FlightId))
                .Select(f => f.FlightId)
                .ToListAsync(cancellationToken);

            if (flightIds.Except(existingFlightIds).Any())
                return ApiResult<BookingDto>.Failure("Một hoặc nhiều chuyến bay không tồn tại");

            var flightSeatPrices = await _unitOfWork.FlightSeatPriceRepository
                .GetByCondition(fsp => flightIds.Contains(fsp.FlightId) && fsp.ClassId == request.ClassId)
                .ToListAsync(cancellationToken);

            if (flightSeatPrices.Count != flightIds.Count)
                return ApiResult<BookingDto>.Failure("Không tìm thấy giá vé cho hạng ghế này");

            var pricePerFlight = flightSeatPrices.ToDictionary(fsp => fsp.FlightId, fsp => fsp.Price);

            var typeIds = request.Passengers
                .Select(p => p.TypeId)
                .Distinct()
                .ToList();

            var passengerTypes = await _unitOfWork.PassengerTypeRepository
                .GetByCondition(pt => typeIds.Contains(pt.TypeId))
                .ToListAsync(cancellationToken);

            if (typeIds.Except(passengerTypes.Select(pt => pt.TypeId)).Any())
                return ApiResult<BookingDto>.Failure("Loại hành khách không hợp lệ");

            var discountPerType = passengerTypes.ToDictionary(pt => pt.TypeId, pt => pt.DiscountRate);
            var passengerCount = request.Passengers.Count;
            var notEnoughSeats = flightSeatPrices.Where(fsp => fsp.AvailableSeats < passengerCount).ToList();

            if (notEnoughSeats.Any())
            {
                var errors = string.Join(", ", notEnoughSeats
                    .Select(fsp => $"Chuyến bay {fsp.FlightId} chỉ còn {fsp.AvailableSeats} chỗ trống"));
                return ApiResult<BookingDto>.Failure(errors);
            }
            var passengers = request.Passengers.Select(p => p.Adapt<Passenger>()).ToList();

            foreach (var passenger in passengers)
                await _unitOfWork.PassengerRepository.AddAsync(passenger);

            var booking = new Domain.Entities.Booking
            {
                UserId = _currentUser.Id.Value,
                ClassId = request.ClassId,
                BookingCode = GenerateBookingCode(),
                BookingDate = DateTime.UtcNow,
                TripType = request.TripType,
                TotalPrice = 0,
                Status = BookingStatus.Pending
            };

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var bookingDetails = new List<BookingDetail>();

            foreach (var flightId in flightIds)
            {
                var basePrice = pricePerFlight[flightId];

                foreach (var (passenger, passengerDto) in passengers.Zip(request.Passengers))
                {
                    var unitPrice = basePrice * (1 - discountPerType[passengerDto.TypeId]);

                    bookingDetails.Add(new BookingDetail
                    {
                        BookingId = booking.BookingId,
                        BookingFlightId = flightId,
                        PassengerId = passenger.PassengerId,
                        UnitPrice = unitPrice
                    });
                }
            }

            foreach (var detail in bookingDetails)
                await _unitOfWork.BookingDetailRepository.AddAsync(detail);

            booking.TotalPrice = bookingDetails.Sum(d => d.UnitPrice);
            _unitOfWork.BookingRepository.Update(booking);

            await _unitOfWork.SaveChangesAsync();
            var bookingDto = booking.Adapt<BookingDto>();

            var passengerDtoByPassengerId = passengers
                .Zip(request.Passengers)
                .ToDictionary(x => x.First.PassengerId, x => x.Second);

            bookingDto.BookingDetails = bookingDetails
                .Select(d => new BookingDetailDto
                {
                    FlightId = d.BookingFlightId,
                    UnitPrice = d.UnitPrice,
                    Passenger = passengerDtoByPassengerId[d.PassengerId]
                })
                .ToList();

            return ApiResult<BookingDto>.Success(bookingDto);
        }

        private static string GenerateBookingCode()
            => Guid.NewGuid().ToString("N")[..6].ToUpper();
    }
}