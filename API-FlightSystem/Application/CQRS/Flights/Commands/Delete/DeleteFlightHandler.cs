using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Flights.Commands.Delete
{
    public class DeleteFlightHandler : IRequestHandler<DeleteFlightCommand, ApiResult<FlightDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFlightHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<FlightDto>> Handle(DeleteFlightCommand request, CancellationToken cancellationToken)
        {
            var flight = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.FlightId == request.FlightId)
                .Include(f => f.Policy)
                .Include(f => f.FlightSegments)
                .Include(f => f.FlightSeatPrices)
                .Include(f => f.FlightServices)
                .FirstOrDefaultAsync(cancellationToken);

            if (flight == null)
                return ApiResult<FlightDto>.Failure("Chuyến bay không tồn tại");

            var now = DateTime.UtcNow;
            if (flight.Status == FlightStatus.Inactive)
                return ApiResult<FlightDto>.Failure("Chuyến bay đã bị vô hiệu hóa trước đó");
            if (now >= flight.DepartureTime && now <= flight.ArrivalTime)
                return ApiResult<FlightDto>.Failure("Không thể xóa chuyến bay đang trong hành trình");
            if (now > flight.ArrivalTime)
                return ApiResult<FlightDto>.Failure("Không thể xóa chuyến bay đã kết thúc");
            if (now >= flight.DepartureTime.AddHours(-24))
                return ApiResult<FlightDto>.Failure("Không thể xóa chuyến bay trong vòng 24 giờ trước khởi hành");

            var paidBooking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingDetails.Any(bd => bd.BookingFlightId == request.FlightId)
                                  && b.Status == BookingStatus.Confirmed)
                .AnyAsync(cancellationToken);
            if (paidBooking)
                return ApiResult<FlightDto>.Failure("Không thể xóa chuyến bay đang có đặt chỗ");

            flight.Status = FlightStatus.Inactive;

            _unitOfWork.FlightRepository.Update(flight);
            var flightDto = flight.Adapt<FlightDto>();
            return ApiResult<FlightDto>.Success(flightDto);
        }
    }
}
