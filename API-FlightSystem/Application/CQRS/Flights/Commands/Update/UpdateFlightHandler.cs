using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Flights.Commands.Update
{
    public class UpdateFlightHandler : IRequestHandler<UpdateFlightCommand, ApiResult<FlightDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFlightHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<FlightDto>> Handle(UpdateFlightCommand request, CancellationToken cancellationToken)
        {
            var flight = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.FlightId == request.FlightId)
                .Include(f => f.FlightSegments)
                .Include(f => f.FlightSeatPrices)
                .Include(f => f.FlightServices)
                .FirstOrDefaultAsync();

            if (flight == null)
                return ApiResult<FlightDto>.Failure(["Chuyến bay không tồn tại"]);

            var now = DateTime.UtcNow;
            if (now >= flight.DepartureTime && now <= flight.ArrivalTime)
                return ApiResult<FlightDto>.Failure(["Không thể cập nhật chuyến bay đang trong hành trình"]);
            if (now > flight.ArrivalTime)
                return ApiResult<FlightDto>.Failure(["Không thể cập nhật chuyến bay đã kết thúc"]);
            if (now >= flight.DepartureTime.AddHours(-24))
                return ApiResult<FlightDto>.Failure(["Không thể cập nhật chuyến bay trong vòng 24 giờ trước khởi hành"]);

            var plane = await _unitOfWork.PlaneRepository.GetByIdAsync(request.PlaneId);
            if (plane == null)
                return ApiResult<FlightDto>.Failure(["Máy bay không tồn tại"]);
            if (plane.Status == FlightStatus.Inactive)
                return ApiResult<FlightDto>.Failure(["Máy bay đang không hoạt động"]);

            var route = await _unitOfWork.RouteRepository.GetByIdAsync(request.RouteId);
            if (route == null)
                return ApiResult<FlightDto>.Failure(["Tuyến bay không tồn tại"]);
            if (route.Status == FlightStatus.Inactive)
                return ApiResult<FlightDto>.Failure(["Tuyến bay đang không hoạt động"]);

            var policy = await _unitOfWork.PolicyRepository
                .GetByCondition(p => p.IsRefund == request.IsRefund && p.IsChange == request.IsChange)
                .FirstOrDefaultAsync();
            if (policy == null)
                return ApiResult<FlightDto>.Failure(["Chính sách không tồn tại"]);

            var hasPaidBooking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingDetails.Any(bd => bd.FlightId == request.FlightId)
                                  && b.Status == BookingStatus.Confirmed)
                .AnyAsync();
            if (hasPaidBooking)
                return ApiResult<FlightDto>.Failure(["Không thể cập nhật chuyến bay đang có đặt vé"]);

            if (request.Segments.Count > 0)
            {
                var segmentRouteIds = request.Segments
                    .Select(s => s.RouteId).Distinct().ToList();

                var existingRouteIds = await _unitOfWork.RouteRepository
                    .GetByCondition(r => segmentRouteIds.Contains(r.RouteId) && r.Status == FlightStatus.Active)
                    .Select(r => r.RouteId)
                    .ToListAsync();

                var invalidRouteIds = segmentRouteIds.Except(existingRouteIds).ToList();
                if (invalidRouteIds.Count > 0)
                    return ApiResult<FlightDto>.Failure(["Một hoặc nhiều tuyến bay của chặng không hợp lệ"]);
            }

            Dictionary<int, Route> segmentRoutes = new();
            if (request.Segments.Count > 0)
            {
                var segmentRouteIds = request.Segments
                    .Select(s => s.RouteId)
                    .Distinct()
                    .ToList();

                segmentRoutes = await _unitOfWork.RouteRepository
                    .GetByCondition(r => segmentRouteIds.Contains(r.RouteId) && r.Status == FlightStatus.Active)
                    .ToDictionaryAsync(r => r.RouteId, cancellationToken);

                var invalidRouteIds = segmentRouteIds.Except(segmentRoutes.Keys).ToList();
                if (invalidRouteIds.Count > 0)
                    return ApiResult<FlightDto>.Failure(["Một hoặc nhiều tuyến bay của chặng không hợp lệ"]);
            }

            flight.PlaneId = request.PlaneId;
            flight.RouteId = request.RouteId;
            flight.PolicyId = policy.PolicyId;
            flight.DepartureTime = request.DepartureTime;
            flight.ArrivalTime = request.DepartureTime.AddMinutes(route.FlightDuration);

            var requestSegmentIds = request.Segments.Where(s => s.SegmentId > 0).Select(s => s.SegmentId).ToList();
            var segmentsToDelete = flight.FlightSegments.Where(s => !requestSegmentIds.Contains(s.SegmentId)).ToList();
            foreach (var seg in segmentsToDelete)
                flight.FlightSegments.Remove(seg);

            foreach (var (seg, index) in request.Segments.Select((s, i) => (s, i)))
            {
                if (seg.SegmentId > 0)
                {
                    var existing = flight.FlightSegments.FirstOrDefault(s => s.SegmentId == seg.SegmentId);
                    if (existing != null)
                    {
                        existing.RouteId = seg.RouteId;
                        existing.DepartureTime = seg.DepartureTime;
                        existing.ArrivalTime = seg.DepartureTime.AddMinutes(route.FlightDuration);
                        existing.SegmentOrder = index + 1;
                    }
                }
                else
                {
                    flight.FlightSegments.Add(new FlightSegment
                    {
                        FlightId = flight.FlightId,
                        RouteId = seg.RouteId,
                        DepartureTime = seg.DepartureTime,
                        ArrivalTime = seg.DepartureTime.AddMinutes(route.FlightDuration),
                        SegmentOrder = index + 1
                    });
                }
            }

            foreach (var seatPrice in flight.FlightSeatPrices)
            {
                var newPrice = request.SeatPrices.FirstOrDefault(p => p.ClassId == seatPrice.ClassId);
                if (newPrice != null)
                    seatPrice.Price = newPrice.Price;
            }

            var requestServiceIds = request.Services.Where(s => s.ServiceId > 0).Select(s => s.ServiceId).ToList();
            var servicesToDelete = flight.FlightServices.Where(s => !requestServiceIds.Contains(s.ServiceId)).ToList();
            foreach (var service in servicesToDelete)
                flight.FlightServices.Remove(service);

            var existingServiceIds2 = flight.FlightServices.Select(s => s.ServiceId).ToList();

            foreach (var service in request.Services.Where(s => !existingServiceIds2.Contains(s.ServiceId)))
            {
                flight.FlightServices.Add(new FlightService
                {
                    FlightId = flight.FlightId,
                    ServiceId = service.ServiceId
                });
            }

            _unitOfWork.FlightRepository.Update(flight);

            flight.Policy = policy;
            return ApiResult<FlightDto>.Success(flight.Adapt<FlightDto>());
        }
    }
}
