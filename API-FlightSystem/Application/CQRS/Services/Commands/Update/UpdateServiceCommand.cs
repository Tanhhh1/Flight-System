using Application.Common;
using Application.CQRS.Services.DTOs;
using MediatR;

namespace Application.CQRS.Services.Commands.Update
{
    public class UpdateServiceCommand : IRequest<ApiResult<ServiceDto>>
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
