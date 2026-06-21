using Application.Common;
using Application.CQRS.Accounts.DTOs;
using MediatR;

namespace Application.CQRS.Accounts.Queries.GetAll
{
    public class GetAllAccountQuery : IRequest<ApiResult<PageList<AccountDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public string? RoleName { get; set; }
    }
}
