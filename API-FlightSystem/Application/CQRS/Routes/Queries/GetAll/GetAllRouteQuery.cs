using Application.Common;
using Application.CQRS.Routes.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Routes.Queries.GetAll
{
    public class GetAllRouteQuery : IRequest<ApiResult<PageList<RouteDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? OriginAirportCode { get; set; }
        public string? DestinationAirportCode { get; set; }
        public FlightStatus? Status { get; set; }
    }
}
