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
                .GetByCondition()
                .Include(f => f.FlightSegments)
                .Include(f => f.FlightSeatPrices)
                .Include(f => f.FlightServices)
                .FirstOrDefaultAsync(f => f.FlightId == request.FlightId, cancellationToken);
            if (flight == null)
                return ApiResult<FlightDto>.Failure("Chuyến bay không tồn tại");

            if (flight.Status == FlightStatus.Cancelled)
                return ApiResult<FlightDto>.Failure("Không thể cập nhật chuyến bay đã bị hủy");

            var now = DateTime.UtcNow;
            if (now >= flight.DepartureTime && now <= flight.ArrivalTime)
                return ApiResult<FlightDto>.Failure("Không thể cập nhật chuyến bay đang trong hành trình");
            if (now > flight.ArrivalTime)
                return ApiResult<FlightDto>.Failure("Không thể cập nhật chuyến bay đã kết thúc");
            if (now >= flight.DepartureTime.AddHours(-24))
                return ApiResult<FlightDto>.Failure("Không thể cập nhật chuyến bay trong vòng 24 giờ trước khởi hành");

            var hasPaidBooking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingDetails.Any(bd => bd.FlightId == request.FlightId)
                                  && b.Status == BookingStatus.Confirmed)
                .AnyAsync(cancellationToken);
            if (hasPaidBooking)
                return ApiResult<FlightDto>.Failure("Không thể cập nhật chuyến bay đang có đặt vé");

            var plane = await _unitOfWork.PlaneRepository.GetByIdAsync(request.PlaneId);
            if (plane == null)
                return ApiResult<FlightDto>.Failure("Máy bay không tồn tại");
            if (plane.Status == FlightStatus.Inactive)
                return ApiResult<FlightDto>.Failure("Máy bay đang không hoạt động");

            var route = await _unitOfWork.RouteRepository.GetByIdAsync(request.RouteId);
            if (route == null)
                return ApiResult<FlightDto>.Failure("Tuyến bay không tồn tại");
            if (route.Status == FlightStatus.Inactive)
                return ApiResult<FlightDto>.Failure("Tuyến bay đang không hoạt động");

            var arrivalTime = request.DepartureTime.AddMinutes(route.FlightDuration);

            var isPlaneConflict = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.PlaneId == request.PlaneId
                    && f.FlightId != request.FlightId
                    && f.Status != FlightStatus.Cancelled
                    && f.DepartureTime < arrivalTime
                    && f.ArrivalTime > request.DepartureTime)
                .AnyAsync(cancellationToken);
            if (isPlaneConflict)
                return ApiResult<FlightDto>.Failure("Máy bay đã được sử dụng trong khoảng thời gian này");

            var isRouteConflict = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.RouteId == request.RouteId
                    && f.FlightId != request.FlightId
                    && f.Status != FlightStatus.Cancelled
                    && f.DepartureTime == request.DepartureTime)
                .AnyAsync(cancellationToken);
            if (isRouteConflict)
                return ApiResult<FlightDto>.Failure("Tuyến bay này đã có chuyến bay vào thời điểm đó");

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
                    return ApiResult<FlightDto>.Failure("Một hoặc nhiều tuyến bay của chặng không hợp lệ");

                foreach (var segment in request.Segments)
                {
                    if (segment.DepartureTime < request.DepartureTime)
                        return ApiResult<FlightDto>.Failure("Thời gian chặng không được trước giờ khởi hành chuyến bay");
                }

                if (request.Segments.Count > 1)
                {
                    for (int i = 0; i < request.Segments.Count - 1; i++)
                    {
                        var current = request.Segments[i];
                        var next = request.Segments[i + 1];
                        var currentRoute = segmentRoutes[current.RouteId];
                        var nextRoute = segmentRoutes[next.RouteId];
                        var currentArrival = current.DepartureTime.AddMinutes(currentRoute.FlightDuration);

                        if (next.DepartureTime < currentArrival)
                            return ApiResult<FlightDto>.Failure("Thời gian các chặng bị chồng lấp nhau");

                        if (currentRoute.DestinationAirportId != nextRoute.OriginAirportId)
                            return ApiResult<FlightDto>.Failure("Sân bay đến của chặng trước phải trùng sân bay đi của chặng sau");
                    }
                }
                var lastSegment = request.Segments[^1];
                var firstSegmentRoute = segmentRoutes[request.Segments[0].RouteId];
                var lastSegmentRoute = segmentRoutes[lastSegment.RouteId];
                arrivalTime = lastSegment.DepartureTime.AddMinutes(lastSegmentRoute.FlightDuration);

                if (firstSegmentRoute.OriginAirportId != route.OriginAirportId)
                    return ApiResult<FlightDto>.Failure("Sân bay đi của chặng đầu phải khớp với điểm đi của tuyến bay chính");

                if (lastSegmentRoute.DestinationAirportId != route.DestinationAirportId)
                    return ApiResult<FlightDto>.Failure("Sân bay đến của chặng cuối phải khớp với điểm đến của tuyến bay chính");
            }

            var policy = await _unitOfWork.PolicyRepository
                .GetByCondition(p => p.IsRefund == request.IsRefund && p.IsChange == request.IsChange)
                .FirstOrDefaultAsync(cancellationToken);
            if (policy == null)
                return ApiResult<FlightDto>.Failure("Chính sách không tồn tại");

            if (request.Services.Count > 0)
            {
                var serviceIds = request.Services
                    .Select(s => s.ServiceId)
                    .Distinct()
                    .ToList();

                var existingServiceIds = await _unitOfWork.ServiceRepository
                    .GetByCondition(s => serviceIds.Contains(s.ServiceId))
                    .Select(s => s.ServiceId)
                    .ToListAsync(cancellationToken);

                var invalidServiceIds = serviceIds.Except(existingServiceIds).ToList();
                if (invalidServiceIds.Count > 0)
                    return ApiResult<FlightDto>.Failure("Một hoặc nhiều dịch vụ không hợp lệ");
            }

            flight.PlaneId = request.PlaneId;
            flight.RouteId = request.RouteId;
            flight.PolicyId = policy.PolicyId;
            flight.DepartureTime = request.DepartureTime;
            flight.ArrivalTime = arrivalTime;

            var requestSegmentIds = request.Segments
                .Where(s => s.SegmentId > 0)
                .Select(s => s.SegmentId)
                .ToList();
            var segmentsToDelete = flight.FlightSegments
                .Where(s => !requestSegmentIds.Contains(s.SegmentId))
                .ToList();
            foreach (var seg in segmentsToDelete)
                flight.FlightSegments.Remove(seg);

            foreach (var (seg, index) in request.Segments.Select((s, i) => (s, i)))
            {
                var segmentRoute = segmentRoutes.GetValueOrDefault(seg.RouteId);
                var duration = segmentRoute?.FlightDuration ?? route.FlightDuration;

                if (seg.SegmentId > 0)
                {
                    var existing = flight.FlightSegments.FirstOrDefault(s => s.SegmentId == seg.SegmentId);
                    if (existing != null)
                    {
                        existing.RouteId = seg.RouteId;
                        existing.DepartureTime = seg.DepartureTime;
                        existing.ArrivalTime = seg.DepartureTime.AddMinutes(duration);
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
                        ArrivalTime = seg.DepartureTime.AddMinutes(duration),
                        SegmentOrder = index + 1,
                    });
                }
            }

            foreach (var seatPrice in flight.FlightSeatPrices)
            {
                var newPrice = request.SeatPrices.FirstOrDefault(p => p.ClassId == seatPrice.ClassId);
                if (newPrice != null)
                    seatPrice.Price = newPrice.Price;
            }

            var requestServiceIds = request.Services.Select(s => s.ServiceId).ToList();

            var servicesToDelete = flight.FlightServices
                .Where(s => !requestServiceIds.Contains(s.ServiceId))
                .ToList();
            foreach (var service in servicesToDelete)
                flight.FlightServices.Remove(service);

            var existingServiceIds2 = flight.FlightServices.Select(s => s.ServiceId).ToList();
            foreach (var service in request.Services.Where(s => !existingServiceIds2.Contains(s.ServiceId)))
            {
                flight.FlightServices.Add(new FlightService
                {
                    FlightId = flight.FlightId,
                    ServiceId = service.ServiceId,
                });
            }

            _unitOfWork.FlightRepository.Update(flight);
            flight.Policy = policy;
            return ApiResult<FlightDto>.Success(flight.Adapt<FlightDto>());
        }
    }
}