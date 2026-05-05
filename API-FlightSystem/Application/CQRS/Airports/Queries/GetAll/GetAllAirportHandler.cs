using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.CQRS.Airports.Queries.GetAll
{
    public class GetAllAirportHandler : IRequestHandler<GetAllAirportQuery, ApiResult<List<AirportDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllAirportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<List<AirportDto>>> Handle(GetAllAirportQuery request, CancellationToken cancellationToken)
        {
            var airports = await _unitOfWork.AirportRepository.GetAllAsync();
            var airportDtos = airports.Adapt<List<AirportDto>>();
            return ApiResult<List<AirportDto>>.Success(airportDtos);
        }
    }
}
