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
            var inc = request.Include;

            var airports = inc.Contains(DataSearch.Airports)
                ? await _unitOfWork.AirportRepository.GetByCondition()
                    .AsNoTracking()
                    .Where(a => a.Status == FlightStatus.Active)
                    .ProjectToType<DataAirportDto>()
                    .ToListAsync(cancellationToken)
                : null;

            var airlines = inc.Contains(DataSearch.Airlines)
                ? await _unitOfWork.AirlineRepository.GetByCondition()
                    .AsNoTracking()
                    .Where(a => a.Status == FlightStatus.Active)
                    .ProjectToType<DataAirlineDto>()
                    .ToListAsync(cancellationToken)
                : null;

            var services = inc.Contains(DataSearch.Services)
                ? await _unitOfWork.ServiceRepository.GetByCondition()
                    .AsNoTracking()
                    .Where(a => a.IsActive)
                    .ProjectToType<DataServiceDto>()
                    .ToListAsync(cancellationToken)
                : null;

            var planes = inc.Contains(DataSearch.Planes)
                ? await _unitOfWork.PlaneRepository.GetByCondition()
                    .AsNoTracking()
                    .Where(a => a.Status == FlightStatus.Active)
                    .ProjectToType<DataPlaneDto>()
                    .ToListAsync(cancellationToken)
                : null;

            return ApiResult<DataSearchDto>.Success(new DataSearchDto
            {
                Airports = airports,
                Airlines = airlines,
                Services = services,
                Planes = planes,
            });
        }
    }
}
