using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Routes.Commands.Update
{
    public class UpdateRouteHandler : IRequestHandler<UpdateRouteCommand, ApiResult<RouteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateRouteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<RouteDto>> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _unitOfWork.RouteRepository.GetByIdAsync(request.RouteId);
            if (route == null)
                return ApiResult<RouteDto>.Failure(["Tuyến bay không tồn tại"]);

            var originAirport = await _unitOfWork.AirportRepository.GetByIdAsync(request.OriginAirportId);
            if (originAirport == null)
                return ApiResult<RouteDto>.Failure(["Sân bay đi không tồn tại"]);
            if (originAirport.Status == FlightStatus.Inactive)
                return ApiResult<RouteDto>.Failure(["Sân bay đi đã ngừng hoạt động"]);

            var destinationAirport = await _unitOfWork.AirportRepository.GetByIdAsync(request.DestinationAirportId);
            if (destinationAirport == null)
                return ApiResult<RouteDto>.Failure(["Sân bay đến không tồn tại"]);
            if (destinationAirport.Status == FlightStatus.Inactive)
                return ApiResult<RouteDto>.Failure(["Sân bay đến đã ngừng hoạt động"]);

            bool isDuplicateRoute = await _unitOfWork.RouteRepository
                .GetByCondition(r => r.OriginAirportId == request.OriginAirportId
                                  && r.DestinationAirportId == request.DestinationAirportId
                                  && r.RouteId != request.RouteId)
                .AnyAsync(cancellationToken);
            if (isDuplicateRoute)
                return ApiResult<RouteDto>.Failure(["Tuyến bay này đã tồn tại"]);

            bool isChangingAirports = route.OriginAirportId != request.OriginAirportId || route.DestinationAirportId != request.DestinationAirportId;
            if (isChangingAirports)
            {
                bool hasActiveOrDelayedFlight = await _unitOfWork.FlightRepository
                    .GetByCondition(f => f.RouteId == request.RouteId && (f.Status == FlightStatus.Active || f.Status == FlightStatus.Delayed))
                    .AnyAsync(cancellationToken);
                if (hasActiveOrDelayedFlight)
                    return ApiResult<RouteDto>.Failure(["Không thể thay đổi tuyến bay khi đang có chuyến bay hoạt động"]);
            }

            bool isChangingDuration = route.FlightDuration != request.FlightDuration;
            if (isChangingDuration)
            {
                bool hasActiveFlight = await _unitOfWork.FlightRepository
                    .GetByCondition(f => f.RouteId == request.RouteId && f.Status == FlightStatus.Active)
                    .AnyAsync(cancellationToken);
                if (hasActiveFlight)
                    return ApiResult<RouteDto>.Failure(["Không thể thay đổi thời gian bay khi đang có chuyến bay hoạt động"]);
            }

            bool hasActiveSegment = await _unitOfWork.FlightSegmentRepository
                .GetByCondition(s => s.RouteId == request.RouteId && s.Flight.Status != FlightStatus.Cancelled && s.Flight.ArrivalTime > DateTime.UtcNow)
                .AnyAsync(cancellationToken);
            if (hasActiveSegment)
                return ApiResult<RouteDto>.Failure(["Không thể sửa tuyến bay khi đang có chặng dừng sử dụng."]);

            request.Adapt(route);
            route.OriginAirport = originAirport;
            route.DestinationAirport = destinationAirport;
            _unitOfWork.RouteRepository.Update(route);
            return ApiResult<RouteDto>.Success(route.Adapt<RouteDto>());
        }
    }
}
