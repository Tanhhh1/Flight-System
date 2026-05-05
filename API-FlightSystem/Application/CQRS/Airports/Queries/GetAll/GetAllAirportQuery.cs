using Application.Common;
using Application.CQRS.Airports.DTOs;
using MediatR;

namespace Application.CQRS.Airports.Queries.GetAll
{
    public class GetAllAirportQuery : IRequest<ApiResult<List<AirportDto>>>
    {
    }
}
