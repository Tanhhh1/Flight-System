using Application.Common;
using Application.CQRS.Services.DTOs;
using MediatR;

namespace Application.CQRS.Services.Commands.Create
{
    public class CreateServiceCommand : IRequest<ApiResult<ServiceDto>>
    {
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
