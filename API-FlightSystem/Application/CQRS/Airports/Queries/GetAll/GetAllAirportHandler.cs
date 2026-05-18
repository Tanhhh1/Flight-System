using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var airport = _unitOfWork.AirportRepository.GetByCondition().AsNoTracking();

            if (!string.IsNullOrEmpty(request.Search))
                airport = airport.Where(a =>
                    a.AirportCode.Contains(request.Search) ||
                    a.AirportName.Contains(request.Search));

            if (!string.IsNullOrEmpty(request.City))
                airport = airport.Where(a => a.City == request.City);

            if (!string.IsNullOrEmpty(request.Country))
                airport = airport.Where(a => a.Country == request.Country);

            if (request.Status.HasValue)
                airport = airport.Where(a => a.Status == request.Status.Value);

            airport = airport.OrderBy(a => a.AirportId);

            // ProjectToType - map ngay trong SQL, chưa load về memory
            var pagedList = await PageList<AirportDto>.ToPagedListAsync(
                airport.ProjectToType<AirportDto>(), // chỉ lấy các cột trong Dto
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<AirportDto>>.Success(pagedList);
        }
    }
}