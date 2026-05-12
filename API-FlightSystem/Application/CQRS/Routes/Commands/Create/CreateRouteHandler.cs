using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.CQRS.Routes.Commands.Create
{
    public class CreateRouteHandler : IRequestHandler<CreateRouteCommand, ApiResult<RouteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateRouteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<RouteDto>> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
        {
            var originAirport = await _unitOfWork.AirportRepository.GetByIdAsync(request.OriginAirportId);
            if (originAirport == null)
                return ApiResult<RouteDto>.Failure(["Sân bay đi không tồn tại"]);

            var destinationAirport = await _unitOfWork.AirportRepository.GetByIdAsync(request.DestinationAirportId);
            if (destinationAirport == null)
                return ApiResult<RouteDto>.Failure(["Sân bay đến không tồn tại"]);

            var existingRoute = _unitOfWork.RouteRepository
                .GetByCondition(r => r.OriginAirportId == request.OriginAirportId
                                  && r.DestinationAirportId == request.DestinationAirportId)
                .FirstOrDefault();

            if (existingRoute != null)
                return ApiResult<RouteDto>.Failure(["Tuyến bay này đã tồn tại"]);

            var route = request.Adapt<Route>();
            await _unitOfWork.RouteRepository.AddAsync(route);
            await _unitOfWork.SaveChangesAsync();

            route.OriginAirport = originAirport;
            route.DestinationAirport = destinationAirport;
            var routeDto = route.Adapt<RouteDto>();

            return ApiResult<RouteDto>.Success(routeDto);
        }
    }
}
