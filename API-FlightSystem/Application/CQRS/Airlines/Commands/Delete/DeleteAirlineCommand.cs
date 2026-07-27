using Application.Common;
using Application.CQRS.Airlines.DTOs;
using MediatR;

namespace Application.CQRS.Airlines.Commands.Delete
{
    public class DeleteAirlineCommand : IRequest<ApiResult<AirlineDto>>
    {
        public int AirlineId { get; set; }
    }
}
