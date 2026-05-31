using Application.Common;
using Application.CQRS.Flights.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Flights.Queries.Search
{
    public class DataSearchQuery : IRequest<ApiResult<DataSearchDto>>
    {
        public HashSet<DataSearch> Include { get; set; } = [];
    }
}
