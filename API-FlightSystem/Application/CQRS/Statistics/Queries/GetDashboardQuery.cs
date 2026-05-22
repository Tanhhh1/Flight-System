using Application.Common;
using Application.CQRS.Statistics.DTOs;
using MediatR;

namespace Application.CQRS.Statistics.Queries
{
    public class GetDashboardQuery : IRequest<ApiResult<StatisticDtos>>
    {
    }
}
