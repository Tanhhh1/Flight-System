using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.CQRS.Airports.Queries.GetById
{
    public class GetByAirportIdHandler : IRequestHandler<GetByAirportIdQuery, ApiResult<AirportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByAirportIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AirportDto>> Handle(GetByAirportIdQuery request, CancellationToken cancellationToken)
        {
            var airport = await _unitOfWork.AirportRepository.GetByIdAsync(request.AirportId);

            if (airport == null)
                return ApiResult<AirportDto>.Failure(["Sân bay không tồn tại"]);

            var airportDto = airport.Adapt<AirportDto>();
            return ApiResult<AirportDto>.Success(airportDto);
        }
    }
}
