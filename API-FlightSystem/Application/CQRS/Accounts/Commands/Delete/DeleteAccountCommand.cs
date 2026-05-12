using Application.Common;
using Application.CQRS.Accounts.DTOs;
using MediatR;

namespace Application.CQRS.Accounts.Commands.Delete
{
    public class DeleteAccountCommand : IRequest<ApiResult<AccountDto>>
    {
        public int UserId { get; set; }
    }
}
