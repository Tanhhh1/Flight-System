using Application.Common;
using Application.CQRS.Airlines.DTOs;
using MediatR;

namespace Application.CQRS.Airlines.Queries.GetAll
{
    public class GetAllAirlineQuery : IRequest<ApiResult<PageList<AirlineDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
    }
}
