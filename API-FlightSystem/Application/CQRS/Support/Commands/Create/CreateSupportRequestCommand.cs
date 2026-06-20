using Application.Common;
using Application.CQRS.Support.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Support.Commands.Create
{
    public class CreateSupportRequestCommand : IRequest<ApiResult<SupportRequestDto>>
    {
        public int BookingId { get; set; }
        public RequestType RequestType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int? NewFlightId { get; set; }
    }
}
