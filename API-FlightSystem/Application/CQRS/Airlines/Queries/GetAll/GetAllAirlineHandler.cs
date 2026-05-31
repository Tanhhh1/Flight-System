using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Airlines.Queries.GetAll
{
    public class GetAllAirlineHandler : IRequestHandler<GetAllAirlineQuery, ApiResult<PageList<AirlineDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllAirlineHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<AirlineDto>>> Handle(GetAllAirlineQuery request, CancellationToken cancellationToken)
        {
            var airline = _unitOfWork.AirlineRepository.GetByCondition().AsNoTracking();
            if (!string.IsNullOrEmpty(request.Search))
            {
                var keyword = request.Search.Trim().ToLower();
                airline = airline.Where(u => u.AirlineCode!.ToLower().Contains(keyword) || u.AirlineName.ToLower().Contains(keyword));
            }

            if (request.Status.HasValue)
                airline = airline.Where(a => a.Status == request.Status.Value);

            airline = airline.OrderBy(a => a.AirlineId);

            var pagedList = await PageList<AirlineDto>.ToPagedListAsync(
                airline.ProjectToType<AirlineDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<AirlineDto>>.Success(pagedList);
        }
    }
}
