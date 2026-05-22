using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Routes.Commands.Delete
{
    public class DeleteRouteCommand : IRequest<ApiResult<RouteDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Route"];
        public int RouteId { get; set; }
    }
}
