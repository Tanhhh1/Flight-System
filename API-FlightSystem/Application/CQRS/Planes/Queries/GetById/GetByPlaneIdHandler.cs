using Application.Common;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Planes.Queries.GetById
{
    public class GetByPlaneIdHandler : IRequestHandler<GetByPlaneIdQuery, ApiResult<PlaneDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetByPlaneIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PlaneDto>> Handle(GetByPlaneIdQuery request, CancellationToken cancellationToken)
        {
            var plane = await _unitOfWork.PlaneRepository.GetByCondition()
                .Include(p => p.Airline)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PlaneId == request.PlaneId, cancellationToken);

            if (plane == null)
                return ApiResult<PlaneDto>.Failure("Máy bay không tồn tại");

            var planeDto = plane.Adapt<PlaneDto>();
            return ApiResult<PlaneDto>.Success(planeDto);
        }
    }
}