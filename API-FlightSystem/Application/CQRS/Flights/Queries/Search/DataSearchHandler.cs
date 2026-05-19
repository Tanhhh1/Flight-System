using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Flights.Queries.Search
{
    public class DataSearchHandler : IRequestHandler<DataSearchQuery, ApiResult<DataSearchDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DataSearchHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<DataSearchDto>> Handle(DataSearchQuery request, CancellationToken cancellationToken)
        {
            var airports = await _unitOfWork.AirportRepository
                .GetByCondition()
                .AsNoTracking()
                .Where(a => a.Status == FlightStatus.Active)
                .ProjectToType<DataAirportDto>()
                .ToListAsync(cancellationToken);

            var airlines = await _unitOfWork.AirlineRepository
                .GetByCondition()
                .AsNoTracking()
                .Where(a => a.Status == FlightStatus.Active)
                .ProjectToType<DataAirlineDto>()
                .ToListAsync(cancellationToken);

            var services = await _unitOfWork.ServiceRepository
                .GetByCondition()
                .AsNoTracking()
                .Where(a => a.IsActive == true)
                .ProjectToType<DataServiceDto>()
                .ToListAsync(cancellationToken);

            return ApiResult<DataSearchDto>.Success(new DataSearchDto
            {
                Airports = airports,
                Airlines = airlines,
                Services = services
            });
        }
    }
}
