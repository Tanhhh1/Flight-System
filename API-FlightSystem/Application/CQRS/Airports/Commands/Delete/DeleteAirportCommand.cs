using Application.Common;
using Application.CQRS.Airports.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airports.Commands.Delete
{
    public class DeleteAirportCommand : IRequest<ApiResult<AirportDto>>
    {
        public int AirportId { get; set; }
    }
}
