using Application.Common;
using Application.CQRS.Dashboard.DTOs;
using MediatR;

namespace Application.CQRS.Dashboard.Queries.GetSummary
{
    public class GetDashboardSummaryQuery : IRequest<ApiResult<DashboardSummaryDto>>
    {
    }
}