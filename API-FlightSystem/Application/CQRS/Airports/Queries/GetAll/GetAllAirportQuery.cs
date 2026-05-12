using Application.Common;
using Application.CQRS.Airports.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airports.Queries.GetAll
{
    public class GetAllAirportQuery : IRequest<ApiResult<PageList<AirportDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public FlightStatus? Status { get; set; }
    }
}
