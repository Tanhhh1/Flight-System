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
                return ApiResult<BookingDto>.Failure(["Bạn cần đăng nhập để đặt vé"]);

            var flightIds = request.Details
                .Select(d => d.FlightId)
                .Distinct()
                .ToList();

            var existingFlightIds = await _unitOfWork.FlightRepository
                .GetByCondition(f => flightIds.Contains(f.FlightId))
                .Select(f => f.FlightId)
                .ToListAsync(cancellationToken);

            if (flightIds.Except(existingFlightIds).Any())
                return ApiResult<BookingDto>.Failure(["Một hoặc nhiều chuyến bay không tồn tại"]);

            var flightSeatPrices = await _unitOfWork.FlightSeatPriceRepository
                .GetByCondition(fsp =>
                    flightIds.Contains(fsp.FlightId) &&
                    fsp.ClassId == request.ClassId)
                .ToListAsync(cancellationToken);

            if (flightSeatPrices.Count != flightIds.Count)
                return ApiResult<BookingDto>.Failure(["Không tìm thấy giá vé cho hạng ghế này"]);

            var pricePerFlight = flightSeatPrices
                .ToDictionary(fsp => fsp.FlightId, fsp => fsp.Price);

            var typeIds = request.Details
                .Select(d => d.Passenger.TypeId)
                .Distinct()
                .ToList();

            var passengerTypes = await _unitOfWork.PassengerTypeRepository
                .GetByCondition(pt => typeIds.Contains(pt.TypeId))
                .ToListAsync(cancellationToken);

            var discountPerType = passengerTypes
                .ToDictionary(pt => pt.TypeId, pt => pt.DiscountRate);

            var missingTypeIds = typeIds.Except(discountPerType.Keys).ToList();
            if (missingTypeIds.Any())
                return ApiResult<BookingDto>.Failure(["Loại hành khách không hợp lệ"]);

            var passengerCountPerFlight = request.Details
                .GroupBy(d => d.FlightId)
                .ToDictionary(g => g.Key, g => g.Count());

            var notEnoughSeats = flightSeatPrices
               .Where(fsp => fsp.AvailableSeats < passengerCountPerFlight[fsp.FlightId])
               .ToList();
            if (notEnoughSeats.Any())
            {
                var errors = notEnoughSeats
                    .Select(fsp => $"Chuyến bay {fsp.FlightId} chỉ còn {fsp.AvailableSeats} chỗ trống")
                    .ToList();
                return ApiResult<BookingDto>.Failure(errors);
            }

            foreach (var fsp in flightSeatPrices)
            {
                if (fsp.AvailableSeats < passengerCountPerFlight[fsp.FlightId])
                    return ApiResult<BookingDto>.Failure([$"Chuyến bay {fsp.FlightId} không đủ chỗ trống"]);

                fsp.AvailableSeats -= passengerCountPerFlight[fsp.FlightId];
                _unitOfWork.FlightSeatPriceRepository.Update(fsp);
            }

            var passengers = request.Details
                .Select(d => d.Passenger.Adapt<Passenger>())
                .ToList();

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

            var bookingDetails = request.Details
                .Select((d, i) =>
                {
                    var basePrice = pricePerFlight[d.FlightId];
                    var discountRate = discountPerType[d.Passenger.TypeId];
                    var unitPrice = basePrice * (1 - discountRate);

                    return new BookingDetail
                    {
                        BookingId = booking.BookingId,
                        FlightId = d.FlightId,
                        PassengerId = passengers[i].PassengerId,
                        UnitPrice = unitPrice
                    };
                })
                .ToList();

            foreach (var detail in bookingDetails)
                await _unitOfWork.BookingDetailRepository.AddAsync(detail);

            booking.TotalPrice = bookingDetails.Sum(d => d.UnitPrice);
            _unitOfWork.BookingRepository.Update(booking);

            await _unitOfWork.SaveChangesAsync();

            var dto = booking.Adapt<BookingDto>();
            dto.BookingDetails = bookingDetails
                .Select((d, i) =>
                {
                    var detailDto = d.Adapt<BookingDetailDto>();
                    detailDto.Passenger = passengers[i].Adapt<PassengerDto>();
                    return detailDto;
                })
                .ToList();

            return ApiResult<BookingDto>.Success(dto);
        }

        private static string GenerateBookingCode()
            => Guid.NewGuid().ToString("N")[..6].ToUpper();
    }
}