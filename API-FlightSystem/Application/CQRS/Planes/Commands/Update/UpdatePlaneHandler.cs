using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
                return ApiResult<PlaneDto>.Failure("Hãng bay không tồn tại");

            var plane = await _unitOfWork.PlaneRepository.GetByIdAsync(request.PlaneId);
            if (plane == null)
                return ApiResult<PlaneDto>.Failure("Máy bay không tồn tại");

            var unfinishedFlight = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.PlaneId == request.PlaneId
                                  && (f.Status == FlightStatus.Active || f.Status == FlightStatus.Delayed))
                .AnyAsync(cancellationToken);

            if (unfinishedFlight)
                return ApiResult<PlaneDto>.Failure("Không thể cập nhật thông tin máy bay vì vẫn còn chuyến bay chưa hoàn thành");

            request.Adapt(plane);
            _unitOfWork.PlaneRepository.Update(plane);

            var planeDto = plane.Adapt<PlaneDto>();
            return ApiResult<PlaneDto>.Success(planeDto);
        }
    }
}