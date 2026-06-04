using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Routes.Commands.Delete
{
    public class DeleteRouteHandler : IRequestHandler<DeleteRouteCommand, ApiResult<RouteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteRouteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<RouteDto>> Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _unitOfWork.RouteRepository.GetByIdAsync(request.RouteId);
            if (route == null)
                return ApiResult<RouteDto>.Failure("Tuyến bay không tồn tại");

            if (route.Status == FlightStatus.Inactive)
                return ApiResult<RouteDto>.Failure("Tuyến bay đã ngừng hoạt động trước đó");

            var activeFlights = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.RouteId == request.RouteId && (f.Status == FlightStatus.Active || f.Status == FlightStatus.Delayed))
                .AnyAsync(cancellationToken);
            if (activeFlights)
                return ApiResult<RouteDto>.Failure("Tuyến bay đang có chuyến bay hoạt động, không thể vô hiệu hóa");

            var activeSegment = await _unitOfWork.FlightSegmentRepository
                .GetByCondition(s => s.RouteId == request.RouteId
                                  && (s.Flight.Status == FlightStatus.Active || s.Flight.Status == FlightStatus.Delayed))
                .AnyAsync(cancellationToken);
            if (activeSegment)
                return ApiResult<RouteDto>.Failure("Tuyến bay đang nằm trong một chặng dừng của chuyến bay đang hoạt động");

            route.Status = FlightStatus.Inactive;
            _unitOfWork.RouteRepository.Update(route);

            var routeDto = route.Adapt<RouteDto>();
            return ApiResult<RouteDto>.Success(routeDto);
        }
    }
}