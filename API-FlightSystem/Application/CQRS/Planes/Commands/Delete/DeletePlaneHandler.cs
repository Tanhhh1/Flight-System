using Application.Common;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.CQRS.Planes.Commands.Delete
{
    public class DeletePlaneHandler : IRequestHandler<DeletePlaneCommand, ApiResult<PlaneDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeletePlaneHandler(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PlaneDto>> Handle(DeletePlaneCommand request, CancellationToken cancellationToken)
        {
            var plane = await _unitOfWork.PlaneRepository.GetByIdAsync(request.PlaneId);

            if (plane == null)
                return ApiResult<PlaneDto>.Failure(new[] { "Máy bay không tồn tại" });

            if (plane.Status == FlightStatus.Inactive)
                return ApiResult<PlaneDto>.Failure(new[] { "Máy bay đã bị vô hiệu hóa trước đó" });

            plane.Status = FlightStatus.Inactive;

            var planeDto = plane.Adapt<PlaneDto>();
            return ApiResult<PlaneDto>.Success(planeDto);
        }

        /* Kiểm tra có Flight nào đang Active/Delayed không
         * Nếu có → không cho vô hiệu hóa
         * Hoặc tự động Cancelled → tùy nghiệp vụ */
    }
}
