using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Routes.Queries.GetAll
{
    public class GetAllRouteHandler : IRequestHandler<GetAllRouteQuery, ApiResult<PageList<RouteDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllRouteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<RouteDto>>> Handle(GetAllRouteQuery request, CancellationToken cancellationToken)
        {
            var route = _unitOfWork.RouteRepository.GetByCondition()
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .AsNoTracking();

            if (request.Status.HasValue)
                route = route.Where(a => a.Status == request.Status.Value);

            if (!string.IsNullOrWhiteSpace(request.OriginAirportCode))
                route = route.Where(r => r.OriginAirport.AirportCode == request.OriginAirportCode);

            if (!string.IsNullOrWhiteSpace(request.DestinationAirportCode))
                route = route.Where(r => r.DestinationAirport.AirportCode == request.DestinationAirportCode);

            route = route.OrderBy(r => r.RouteId);

            var pagedList = await PageList<RouteDto>.ToPagedListAsync(
                route.ProjectToType<RouteDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<RouteDto>>.Success(pagedList);
        }
    }
}