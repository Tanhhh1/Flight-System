using Application.Common;
using Application.CQRS.Planes.DTOs;
using MediatR;

namespace Application.CQRS.Planes.Queries.GetAll
{
    public class GetAllPlaneQuery : IRequest<ApiResult<PageList<PlaneDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
    }
}
