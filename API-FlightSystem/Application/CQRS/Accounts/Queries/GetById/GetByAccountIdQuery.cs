using Application.Common;
using Application.CQRS.Accounts.DTOs;
using MediatR;

namespace Application.CQRS.Accounts.Queries.GetById
{
    public class GetByAccountIdQuery : IRequest<ApiResult<AccountDto>>
    {
        public int UserId { get; set; }
    }
}
