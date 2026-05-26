using Application.Common;
using Application.CQRS.Accounts.DTOs;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Accounts.Queries.GetAll
{
    public class GetAllAccountHandler : IRequestHandler<GetAllAccountQuery, ApiResult<PageList<AccountDto>>>
    {
        private readonly UserManager<User> _userManager;

        public GetAllAccountHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResult<PageList<AccountDto>>> Handle(GetAllAccountQuery request, CancellationToken cancellationToken)
        {
            var query = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var keyword = request.Search.Trim().ToLower();
                query = query.Where(u => u.UserName!.ToLower().Contains(keyword) || u.Fullname.ToLower().Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(request.RoleName))
            {
                query = query.Where(u =>
                    u.UserRoles.Any(ur =>
                        ur.Role.Name!.ToLower() == request.RoleName.Trim().ToLower()));
            }
            query = query.OrderByDescending(u => u.CreatedAt);

            var result = await PageList<AccountDto>.ToPagedListAsync(
                query.ProjectToType<AccountDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<AccountDto>>.Success(result);
        }
    }
}
