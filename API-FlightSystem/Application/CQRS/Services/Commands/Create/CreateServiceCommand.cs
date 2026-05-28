using Application.Common;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Services.Commands.Create
{
    public class CreateServiceCommand : IRequest<ApiResult<ServiceDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Service"];
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
