using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.CQRS.Planes.Commands.Update
{
    public class UpdatePlaneHandler : IRequestHandler<UpdatePlaneCommand, ApiResult<PlaneDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdatePlaneHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<ApiResult<PlaneDto>> Handle(UpdatePlaneCommand request, CancellationToken cancellationToken)
        {
            var airline = await _unitOfWork.AirlineRepository.GetByIdAsync(request.AirlineId);
            if (airline == null)
                return ApiResult<PlaneDto>.Failure(new[] { "Hãng bay không tồn tại" });

            var plane = await _unitOfWork.PlaneRepository.GetByIdAsync(request.PlaneId);
            if (plane == null) return ApiResult<PlaneDto>.Failure(new[] { "Máy bay không tồn tại" });
            request.Adapt(plane);
            _unitOfWork.PlaneRepository.Update(plane);
            var planeDto = plane.Adapt<PlaneDto>();
            return ApiResult<PlaneDto>.Success(planeDto);
        }

        /* Cập nhật AirlineId (chuyển hãng)
         * Airline mới phải đang Active
         * Không có Flight nào đang Active/Delayed */
    }
}
