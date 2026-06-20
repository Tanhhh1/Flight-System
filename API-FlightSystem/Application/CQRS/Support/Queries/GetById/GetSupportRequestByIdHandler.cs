using Application.Common;
using Application.CQRS.Support.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Support.Queries.GetById
{
    public class GetSupportRequestByIdHandler : IRequestHandler<GetSupportRequestByIdQuery, ApiResult<SupportRequestDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSupportRequestByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<SupportRequestDetailDto>> Handle(GetSupportRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _unitOfWork.SupportRequestRepository
                .GetByCondition(sr => sr.RequestId == request.RequestId)
                .AsNoTracking()
                .ProjectToType<SupportRequestDetailDto>()
                .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
                return ApiResult<SupportRequestDetailDto>.Failure("Không tìm thấy yêu cầu hỗ trợ");

            return ApiResult<SupportRequestDetailDto>.Success(dto);
        }
    }
}
