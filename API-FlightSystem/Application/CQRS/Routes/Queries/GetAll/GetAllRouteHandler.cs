using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

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
            var route = _unitOfWork.RouteRepository.GetByCondition();

            if (!string.IsNullOrEmpty(request.Search))
                route = route.Where(r =>
                    r.OriginAirport.AirportName.Contains(request.Search) ||
                    r.OriginAirport.AirportCode.Contains(request.Search) ||
                    r.DestinationAirport.AirportName.Contains(request.Search) ||
                    r.DestinationAirport.AirportCode.Contains(request.Search));

            if (request.Status.HasValue)
                route = route.Where(a => a.Status == request.Status.Value);

            if (!string.IsNullOrEmpty(request.OriginCity))
                route = route.Where(r => r.OriginAirport.City == request.OriginCity);

            if (!string.IsNullOrEmpty(request.DestinationCity))
                route = route.Where(r => r.DestinationAirport.City == request.DestinationCity);

            route = route.OrderBy(r => r.RouteId);

            var pagedList = await PageList<RouteDto>.ToPagedListAsync(
                route.ProjectToType<RouteDto>(),
                request.PageIndex,
                request.PageSize
            );

            return ApiResult<PageList<RouteDto>>.Success(pagedList);
        }
    }
}