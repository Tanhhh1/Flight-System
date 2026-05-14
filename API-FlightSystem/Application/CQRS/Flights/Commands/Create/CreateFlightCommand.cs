using Application.Common;
using Application.CQRS.Flights.DTOs;
using MediatR;

namespace Application.CQRS.Flights.Commands.Create
{
    public class CreateFlightCommand : IRequest<ApiResult<FlightDto>>
    {
        public int PlaneId { get; set; }
        public int RouteId { get; set; }
        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public List<FlightSegmentDto> Segments { get; set; } = [];
        public List<FlightSeatPriceDto> SeatPrices { get; set; } = [];
        public List<FlightServiceDto> Services { get; set; } = [];
    }
}
