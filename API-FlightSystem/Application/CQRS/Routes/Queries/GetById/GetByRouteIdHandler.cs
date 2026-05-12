using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Routes.Queries.GetById
{
    public class GetByRouteIdHandler : IRequestHandler<GetByRouteIdQuery, ApiResult<RouteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByRouteIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<RouteDto>> Handle(GetByRouteIdQuery request, CancellationToken cancellationToken)
        {
            var route = await _unitOfWork.RouteRepository
                .GetByCondition(r => r.RouteId == request.RouteId)
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .FirstOrDefaultAsync(cancellationToken);

            if (route == null)
                return ApiResult<RouteDto>.Failure(["Tuyến bay không tồn tại"]);

            return ApiResult<RouteDto>.Success(route.Adapt<RouteDto>());
        }
    }
}