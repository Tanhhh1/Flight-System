using Application.Common;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Services.Commands.Delete
{
    public class DeleteServiceCommand : IRequest<ApiResult<ServiceDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Service"];
        public int ServiceId { get; set; }
    }
}
