using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.CQRS.Airports.Queries.GetAll
{
    public class GetAllAirportHandler : IRequestHandler<GetAllAirportQuery, ApiResult<PageList<AirportDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllAirportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<AirportDto>>> Handle(GetAllAirportQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.AirportRepository.GetByCondition();

            if (!string.IsNullOrEmpty(request.Search))
                query = query.Where(a =>
                    a.AirportCode.Contains(request.Search) ||
                    a.AirportName.Contains(request.Search));

            if (!string.IsNullOrEmpty(request.City))
                query = query.Where(a => a.City == request.City);

            if (!string.IsNullOrEmpty(request.Country))
                query = query.Where(a => a.Country == request.Country);

            query = query.OrderBy(a => a.AirportId);

            var pagedList = await PageList<AirportDto>.ToPagedListAsync(
                query.ProjectToType<AirportDto>(),
                request.PageIndex,
                request.PageSize
            );

            return ApiResult<PageList<AirportDto>>.Success(pagedList);
        }
    }
}