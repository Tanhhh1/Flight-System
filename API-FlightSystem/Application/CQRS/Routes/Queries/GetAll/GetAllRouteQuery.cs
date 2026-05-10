using Application.Common;
using Application.CQRS.Routes.DTOs;
using MediatR;

namespace Application.CQRS.Routes.Queries.GetAll
{
    public class GetAllRouteQuery : IRequest<ApiResult<PageList<RouteDto>>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? OriginCity { get; set; }
        public string? DestinationCity { get; set; }
    }
}
