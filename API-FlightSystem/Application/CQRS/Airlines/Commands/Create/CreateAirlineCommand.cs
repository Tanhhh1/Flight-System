using Application.Common;
using Application.CQRS.Airlines.DTOs;
using MediatR;

namespace Application.CQRS.Airlines.Commands.Create
{
    public class CreateAirlineCommand : IRequest<ApiResult<AirlineDto>>
    {
        public string AirlineName { get; set; } = string.Empty;
        public string AirlineCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
