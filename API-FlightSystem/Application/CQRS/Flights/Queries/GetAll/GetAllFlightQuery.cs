using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Flights.Queries.GetAll
{
    public class GetAllFlightQuery : IRequest<ApiResult<PageList<FlightListDto>>>, ICacheable, IQuery
    {
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Flight", 5);
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? OriginCity { get; set; }
        public string? DestinationCity { get; set; }
        public DateTime? DepartureDate { get; set; }
        public FlightStatus? Status { get; set; }
        public int? AirlineId { get; set; }
    }
}
