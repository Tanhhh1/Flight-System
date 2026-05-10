using Application.Common;
using Application.CQRS.Airports.DTOs;
using MediatR;

namespace Application.CQRS.Airports.Queries.GetById
{
    public class GetByAirportIdQuery : IRequest<ApiResult<AirportDto>>
    {
        public int AirportId { get; set; }
    }
}
