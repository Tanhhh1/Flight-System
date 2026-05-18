using Application.Common;
using Application.CQRS.Flights.DTOs;
using MediatR;

namespace Application.CQRS.Flights.Queries.Search
{
    public class SearchFlightQuery : IRequest<ApiResult<PageList<FlightSearchDto>>>
    {
        public string OriginCity { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public int? StopCount { get; set; }
        public int? AirlineId { get; set; }
        public int ClassId { get; set; }
        public List<int>? ServiceIds { get; set; }
        public int? DepartureFromHour { get; set; }
        public int? DepartureToHour { get; set; }
        public int? ArrivalFromHour { get; set; }
        public int? ArrivalToHour { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
