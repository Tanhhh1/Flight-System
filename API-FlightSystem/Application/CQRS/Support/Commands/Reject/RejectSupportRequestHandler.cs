using Application.Common;
using Application.CQRS.Support.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Support.Commands.Reject
{
    public class RejectSupportRequestHandler : IRequestHandler<RejectSupportRequestCommand, ApiResult<SupportRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RejectSupportRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResult<SupportRequestDto>> Handle(RejectSupportRequestCommand request, CancellationToken cancellationToken)
        {
            var supportRequest = await _unitOfWork.SupportRequestRepository
                .GetByCondition(sr => sr.RequestId == request.RequestId)
                .FirstOrDefaultAsync(cancellationToken);

            if (supportRequest is null)
                return ApiResult<SupportRequestDto>.Failure("Không tìm thấy yêu cầu hỗ trợ");

            if (supportRequest.Status != SupportStatus.Pending)
                return ApiResult<SupportRequestDto>.Failure("Chỉ có thể từ chối yêu cầu đang chờ xử lý");

            supportRequest.Status = SupportStatus.Rejected;

            _unitOfWork.SupportRequestRepository.Update(supportRequest);

            var dto = supportRequest.Adapt<SupportRequestDto>();
            return ApiResult<SupportRequestDto>.Success(dto);
        }
    }
}
