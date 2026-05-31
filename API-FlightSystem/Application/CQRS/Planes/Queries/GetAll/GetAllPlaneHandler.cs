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
            var plane = _unitOfWork.PlaneRepository
                .GetByCondition() 
                .Include(p => p.Airline)                             
                .AsNoTracking();

            if (!string.IsNullOrEmpty(request.Search))
            {
                var keyword = request.Search.Trim().ToLower();
                plane = plane.Where(p => p.PlaneModel.ToLower().Contains(keyword));
            }

            if (request.Status.HasValue)
                plane = plane.Where(p => p.Status == request.Status.Value);

            plane = plane.OrderBy(p => p.PlaneId);

            var pagedList = await PageList<PlaneDto>.ToPagedListAsync(
                plane.ProjectToType<PlaneDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<PlaneDto>>.Success(pagedList);
        }
    }
}
