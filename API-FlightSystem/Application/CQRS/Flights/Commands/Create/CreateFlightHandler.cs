using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Flights.Commands.Create
{
    public class CreateFlightHandler : IRequestHandler<CreateFlightCommand, ApiResult<FlightDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateFlightHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResult<FlightDto>> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
        {
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

            var policy = await _unitOfWork.PolicyRepository
                .GetByCondition(p => p.IsRefund == request.IsRefund && p.IsChange == request.IsChange)
                .FirstOrDefaultAsync();
            if (policy == null)
                return ApiResult<FlightDto>.Failure(["Chính sách không tồn tại"]);

            if (request.Services.Count > 0)
            {
                var serviceIds = request.Services
                    .Select(s => s.ServiceId)
                    .Distinct()
                    .ToList();

                var existingServiceIds = await _unitOfWork.ServiceRepository
                    .GetByCondition(s => serviceIds.Contains(s.ServiceId))
                    .Select(s => s.ServiceId)
                    .ToListAsync();

                var invalidServiceIds = serviceIds.Except(existingServiceIds).ToList();
                if (invalidServiceIds.Count > 0)
                    return ApiResult<FlightDto>.Failure(["Một hoặc nhiều dịch vụ không hợp lệ"]);
            }

            var seatCountByClass = await _unitOfWork.SeatTemplateRepository
                .GetByCondition()
                .GroupBy(st => st.ClassId)
                .Select(g => new { ClassId = g.Key, Count = g.Count() })
                .ToListAsync();

            var flight = new Flight
            {
                PlaneId = request.PlaneId,
                RouteId = request.RouteId,
                PolicyId = policy.PolicyId,
                DepartureTime = request.DepartureTime,
                ArrivalTime = request.DepartureTime.AddMinutes(route.FlightDuration),
                Status = FlightStatus.Active,

                FlightSegments = request.Segments
                    .Select((s, index) =>
                    {
                        var segmentRoute = segmentRoutes[s.RouteId];
                        return new FlightSegment
                        {
                            RouteId = s.RouteId,
                            DepartureTime = s.DepartureTime,
                            ArrivalTime = s.DepartureTime.AddMinutes(segmentRoute.FlightDuration),
                            SegmentOrder = index + 1,
                        };
                    })
                    .ToList(),

                FlightSeatPrices = request.SeatPrices
                    .Select(p => new FlightSeatPrice
                    {
                        ClassId = p.ClassId,
                        Price = p.Price,
                        AvailableSeats = seatCountByClass
                            .FirstOrDefault(s => s.ClassId == p.ClassId)?.Count ?? 0
                    }).ToList(),

                FlightServices = request.Services
                    .Select(s => new FlightService
                    {
                        ServiceId = s.ServiceId
                    }).ToList()
            };

            await _unitOfWork.FlightRepository.AddAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            flight.Policy = policy;
            var flightDto = flight.Adapt<FlightDto>();
            return ApiResult<FlightDto>.Success(flightDto);
        }
    }
}
