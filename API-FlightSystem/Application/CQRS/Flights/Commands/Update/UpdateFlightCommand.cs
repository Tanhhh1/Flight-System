using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Flights.Commands.Update
{
    public class UpdateFlightCommand : IRequest<ApiResult<FlightDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Flight"];
        public int FlightId { get; set; }
        public int PlaneId { get; set; }
        public int RouteId { get; set; }
        public bool IsRefund { get; set; }
        public bool IsChange { get; set; }
        public DateTime DepartureTime { get; set; }
        public FlightStatus Status { get; set; }
        public List<FlightSegmentDto> Segments { get; set; } = [];
        public List<FlightSeatPriceDto> SeatPrices { get; set; } = [];
        public List<FlightServiceDto> Services { get; set; } = [];
    }
}
