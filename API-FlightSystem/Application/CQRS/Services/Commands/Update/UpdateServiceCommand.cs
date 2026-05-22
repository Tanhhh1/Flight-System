using Application.Common;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Services.Commands.Update
{
    public class UpdateServiceCommand : IRequest<ApiResult<ServiceDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Service"];
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
