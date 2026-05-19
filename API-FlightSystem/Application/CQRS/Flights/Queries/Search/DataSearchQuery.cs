using Application.Common;
using Application.CQRS.Flights.DTOs;
using MediatR;

namespace Application.CQRS.Flights.Queries.Search
{
    public class DataSearchQuery : IRequest<ApiResult<DataSearchDto>>
    {
    }
}
