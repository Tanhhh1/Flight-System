using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.CQRS.Airports.Queries.GetById
{
    public class GetByIdAirportHandler : IRequestHandler<GetByIdAirportQuery, ApiResult<AirportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdAirportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AirportDto>> Handle(GetByIdAirportQuery request, CancellationToken cancellationToken)
        {
            var airport = await _unitOfWork.AirportRepository.GetByIdAsync(request.AirportId);

            if (airport == null)
                return ApiResult<AirportDto>.Failure(new[] { "Airport not found" });

            var airportDto = airport.Adapt<AirportDto>();
            return ApiResult<AirportDto>.Success(airportDto);
        }
    }
}
