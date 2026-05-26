using Application.Common;
using Application.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Auth.Commands.RevokeToken
{
    public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, ApiResult<String>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RevokeTokenHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<string>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _unitOfWork.RefreshTokenRepository
                .GetByCondition(x => x.Token == request.RefreshToken)
                .FirstOrDefaultAsync(cancellationToken);

            if (refreshToken == null)
                return ApiResult<string>.Failure("Token không tồn tại");

            if (refreshToken.InRevoked)
                return ApiResult<string>.Failure("Token đã bị thu hồi trước đó");

            if (refreshToken.ExpiryTime < DateTime.UtcNow)
                return ApiResult<string>.Failure("Token đã hết hạn");

            refreshToken.InRevoked = true;

            _unitOfWork.RefreshTokenRepository.Update(refreshToken);

            return ApiResult<string>.Success("Đăng xuất thành công");
        }
    }
}
