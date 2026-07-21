using Application.Common;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Auth.Commands.RevokeToken
{
    public class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, ApiResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public RevokeTokenHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<ApiResult<bool>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var hashedToken = _tokenService.HashToken(request.RefreshToken);

            var existingToken = await _unitOfWork.RefreshTokenRepository
                .GetByCondition(rt => rt.Token == hashedToken)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingToken is not null && !existingToken.IsRevoked)
            {
                existingToken.IsRevoked = true;
                _unitOfWork.RefreshTokenRepository.Update(existingToken);
            }
            return ApiResult<bool>.Success(true);
        }
    }
}
