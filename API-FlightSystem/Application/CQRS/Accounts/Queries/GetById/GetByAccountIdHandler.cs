using Application.Common;
using Application.CQRS.Accounts.DTOs;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Accounts.Queries.GetById
{
    public class GetByAccountIdHandler : IRequestHandler<GetByAccountIdQuery, ApiResult<AccountDto>>
    {
        private readonly UserManager<User> _userManager;

        public GetByAccountIdHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResult<AccountDto>> Handle(GetByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return ApiResult<AccountDto>.Failure(["Tài khoản không tồn tại"]);

            var accountDto = user.Adapt<AccountDto>();
            return ApiResult<AccountDto>.Success(accountDto);
        }
    }
}
