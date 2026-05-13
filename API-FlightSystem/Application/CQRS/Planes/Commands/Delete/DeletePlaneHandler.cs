using Application.Common;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                return ApiResult<PlaneDto>.Failure(["Máy bay không tồn tại"]);

            if (plane.Status == FlightStatus.Inactive)
                return ApiResult<PlaneDto>.Failure(["Máy bay đã bị vô hiệu hóa trước đó"]);

            var hasActiveFlight = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.PlaneId == request.PlaneId
                                  && (f.Status == FlightStatus.Active || f.Status == FlightStatus.Delayed))
                .AnyAsync();
            if (hasActiveFlight)
                return ApiResult<PlaneDto>.Failure(["Không thể vô hiệu hóa máy bay đang có chuyến bay hoạt động"]);

            plane.Status = FlightStatus.Inactive;

            var planeDto = plane.Adapt<PlaneDto>();
            return ApiResult<PlaneDto>.Success(planeDto);
        }
    }
}
