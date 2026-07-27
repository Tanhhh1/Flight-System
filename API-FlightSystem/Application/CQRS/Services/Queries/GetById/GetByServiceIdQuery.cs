using Application.Common;
using Application.CQRS.Services.DTOs;
using MediatR;

namespace Application.CQRS.Services.Queries.GetById
{
    public class GetByServiceIdQuery : IRequest<ApiResult<ServiceDto>>
    {
        public int ServiceId { get; set; }
    }
}
