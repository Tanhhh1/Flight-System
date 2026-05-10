using Application.Common;
using Application.CQRS.Routes.DTOs;
using MediatR;

namespace Application.CQRS.Routes.Commands.Delete
{
    public class DeleteRouteCommand : IRequest<ApiResult<RouteDto>>
    {
        public int RouteId { get; set; }
    }
}
