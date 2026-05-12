using Application.Common;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.CQRS.Planes.Commands.Create
{
    public class CreatePlaneHandler : IRequestHandler<CreatePlaneCommand, ApiResult<PlaneDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreatePlaneHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PlaneDto>> Handle(CreatePlaneCommand request, CancellationToken cancellationToken)
        {
            var airline = await _unitOfWork.AirlineRepository.GetByIdAsync(request.AirlineId);
            if (airline == null)
                return ApiResult<PlaneDto>.Failure(["Hãng bay không tồn tại"]);
                    
            var plane = request.Adapt<Plane>();
            await _unitOfWork.PlaneRepository.AddAsync(plane);
            await _unitOfWork.SaveChangesAsync();
            var planeDto = plane.Adapt<PlaneDto>();
            return ApiResult<PlaneDto>.Success(planeDto);
        }
    }
}
