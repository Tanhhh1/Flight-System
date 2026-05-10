using Application.Common;
using Application.CQRS.Airlines.DTOs;
using MediatR;

namespace Application.CQRS.Airlines.Queries.GetById
{
    public class GetByAirlineIdQuery : IRequest<ApiResult<AirlineDto>>
    {
        public int AirlineId { get; set; }
    }
}
