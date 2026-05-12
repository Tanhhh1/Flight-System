using Application.Common;
using Application.CQRS.Services.DTOs;
using MediatR;

namespace Application.CQRS.Services.Commands.Delete
{
    public class DeleteServiceCommand : IRequest<ApiResult<ServiceDto>>
    {
        public int ServiceId { get; set; }
    }
}
