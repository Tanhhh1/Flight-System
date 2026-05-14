using Application.Common;
using Application.CQRS.Flights.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Flights.Queries.GetAll
{
    public class GetAllFlightQuery : IRequest<ApiResult<PageList<FlightListDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public int? RouteId { get; set; }
        public int? AirlineId { get; set; }
        public int? PlaneId { get; set; }
        public FlightStatus? Status { get; set; }
        public DateTime? DepartureFrom { get; set; }
        public DateTime? DepartureTo { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsRefund { get; set; }
        public bool? IsChange { get; set; }
    }
}
