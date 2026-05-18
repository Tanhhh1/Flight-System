using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
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

            var flightIds = request.Details.Select(d => d.FlightId).Distinct().ToList();

            var existingFlightIds = await _unitOfWork.FlightRepository
                .GetByCondition(f => flightIds.Contains(f.FlightId))
                .Select(f => f.FlightId)
                .ToListAsync(cancellationToken);

            var invalidFlightIds = flightIds.Except(existingFlightIds).ToList();
            if (invalidFlightIds.Any())
                return ApiResult<BookingDto>.Failure(["Không tìm thấy chuyến bay"]);

            var passengerCountPerFlight = request.Details
                .GroupBy(d => d.FlightId)
                .ToDictionary(g => g.Key, g => g.Count());

            var flightSeatPrices = await _unitOfWork.FlightSeatPriceRepository
                .GetByCondition(fsp => flightIds.Contains(fsp.FlightId) && fsp.ClassId == request.ClassId)
                .ToListAsync(cancellationToken);

            var booking = request.Adapt<Domain.Entities.Booking>();
            booking.BookingCode = GenerateBookingCode();
            booking.BookingDate = DateTime.UtcNow;
            booking.TotalPrice = booking.BookingDetails.Sum(d => d.UnitPrice);
            booking.Status = BookingStatus.Pending;
            booking.UserId = _currentUser.Id.Value;

            foreach (var fsp in flightSeatPrices)
            {
                fsp.AvailableSeats -= passengerCountPerFlight[fsp.FlightId];
                _unitOfWork.FlightSeatPriceRepository.Update(fsp);
            }

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            var dto = booking.Adapt<BookingDto>();
            return ApiResult<BookingDto>.Success(dto);
        }

        private static string GenerateBookingCode()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..4].ToUpper();
            return $"BK-{datePart}-{randomPart}";
        }
    }
}