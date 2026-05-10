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
                return ApiResult<RouteDto>.Failure(new[] { "Tuyến bay không tồn tại" });

            var originAirport = await _unitOfWork.AirportRepository.GetByIdAsync(request.OriginAirportId);
            if (originAirport == null)
                return ApiResult<RouteDto>.Failure(new[] { "Sân bay đi không tồn tại" });

            if (originAirport.Status == FlightStatus.Inactive)
                return ApiResult<RouteDto>.Failure(new[] { "Sân bay đi đã ngừng hoạt động" });

            var destinationAirport = await _unitOfWork.AirportRepository.GetByIdAsync(request.DestinationAirportId);
            if (destinationAirport == null)
                return ApiResult<RouteDto>.Failure(new[] { "Sân bay đến không tồn tại" });

            if (destinationAirport.Status == FlightStatus.Inactive)
                return ApiResult<RouteDto>.Failure(new[] { "Sân bay đến đã ngừng hoạt động" });

            var existingRoute = _unitOfWork.RouteRepository
                .GetByCondition(r => r.OriginAirportId == request.OriginAirportId
                                  && r.DestinationAirportId == request.DestinationAirportId
                                  && r.RouteId != request.RouteId)
                .FirstOrDefault();

            if (existingRoute != null)
                return ApiResult<RouteDto>.Failure(new[] { "Tuyến bay này đã tồn tại" });

            request.Adapt(route);
            route.OriginAirport = originAirport;
            route.DestinationAirport = destinationAirport;

            _unitOfWork.RouteRepository.Update(route);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult<RouteDto>.Success(route.Adapt<RouteDto>());
        }

        /* Không cho đổi OriginAirportId, DestinationAirportId nếu đang có Flight Active/Delayed
         * Chỉ cho cập nhật FlightDuration khi không có Flight Active */
    }
}
