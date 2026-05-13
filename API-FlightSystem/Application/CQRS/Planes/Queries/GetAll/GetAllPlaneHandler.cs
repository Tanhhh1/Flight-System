using Application.Common;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Planes.Queries.GetAll
{
    public class GetAllPlaneHandler : IRequestHandler<GetAllPlaneQuery, ApiResult<PageList<PlaneDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllPlaneHandler (IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<PlaneDto>>> Handle(GetAllPlaneQuery request, CancellationToken cancellationToken)
        {
            var plane = _unitOfWork.PlaneRepository.GetByCondition(include: q => q.Include(p => p.Airline));
            if (!string.IsNullOrEmpty(request.Search))
                plane = plane.Where(p => p.PlaneModel.Contains(request.Search));

            if (request.Status.HasValue)
                plane = plane.Where(a => a.Status == request.Status.Value);

            plane = plane.OrderBy(a => a.PlaneId);

            var pagedList = await PageList<PlaneDto>.ToPagedListAsync(
                plane.ProjectToType<PlaneDto>(), 
                request.PageIndex,
                request.PageSize
            );

            return ApiResult<PageList<PlaneDto>>.Success(pagedList);
        }
    }
}
