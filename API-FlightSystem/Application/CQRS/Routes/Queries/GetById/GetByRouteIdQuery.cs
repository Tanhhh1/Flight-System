using Application.Common;
using Application.CQRS.Routes.DTOs;
using MediatR;

namespace Application.CQRS.Routes.Queries.GetById
{
    public class GetByIdRouteQuery : IRequest<ApiResult<RouteDto>>
    {
        public int RouteId { get; set; }
    }
}