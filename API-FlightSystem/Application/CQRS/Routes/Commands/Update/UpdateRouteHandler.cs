using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;

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

            var existingRoute = _unitOfWork.RouteRepository
                .GetByCondition(r => r.OriginAirportId == request.OriginAirportId
                                  && r.DestinationAirportId == request.DestinationAirportId
                                  && r.RouteId != request.RouteId)
                .FirstOrDefault();

            if (existingRoute != null)
                return ApiResult<RouteDto>.Failure(["Tuyến bay này đã tồn tại"]);

            var activeFlights = _unitOfWork.FlightRepository
                .GetByCondition(f => f.RouteId == request.RouteId && (f.Status == FlightStatus.Active || f.Status == FlightStatus.Delayed))
                .ToList();
            bool isChangingAirports = route.OriginAirportId != request.OriginAirportId || route.DestinationAirportId != request.DestinationAirportId;
            if (isChangingAirports && activeFlights.Any())
                return ApiResult<RouteDto>.Failure(["Không thể thay đổi sân bay khi đang có chuyến bay hoạt động"]);

            bool isChangingDuration = route.FlightDuration != request.FlightDuration;
            if (isChangingDuration && activeFlights.Any(f => f.Status == FlightStatus.Active))
                return ApiResult<RouteDto>.Failure(["Không thể thay đổi thời gian bay khi đang có chuyến bay hoạt động"]);

            request.Adapt(route);
            route.OriginAirport = originAirport;
            route.DestinationAirport = destinationAirport;

            _unitOfWork.RouteRepository.Update(route);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult<RouteDto>.Success(route.Adapt<RouteDto>());
        }
    }
}
