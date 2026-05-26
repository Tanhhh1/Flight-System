using Application.Common;
using Application.CQRS.Accounts.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.CQRS.Accounts.Commands.Delete
{
    public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, ApiResult<AccountDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteAccountHandler(UserManager<User> userManager, ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AccountDto>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.IsAuthenticated && _currentUser.Id == request.UserId)
                return ApiResult<AccountDto>.Failure("Bạn không thể tự khóa tài khoản của chính mình");

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return ApiResult<AccountDto>.Failure("Tài khoản không tồn tại");

            if (!user.IsActive)
                return ApiResult<AccountDto>.Failure("Tài khoản đã bị khóa trước đó");

            var activeTokens = await _unitOfWork.RefreshTokenRepository
                .GetByCondition(t => t.UserId == request.UserId && !t.InRevoked && !t.IsUsed)
                .ToListAsync(cancellationToken);

            if (activeTokens.Any())
            {
                foreach (var token in activeTokens)
                {
                    token.InRevoked = true;
                    _unitOfWork.RefreshTokenRepository.Update(token);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            user.IsActive = false;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApiResult<AccountDto>.Failure(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

            var accountDto = user.Adapt<AccountDto>();
            return ApiResult<AccountDto>.Success(accountDto);
        }
    }
}
