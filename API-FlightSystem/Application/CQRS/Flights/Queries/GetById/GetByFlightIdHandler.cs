using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Flights.Queries.GetById
{
    public class GetByFlightIdHandler : IRequestHandler<GetByFlightIdQuery, ApiResult<FlightDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByFlightIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<FlightDetailDto>> Handle(GetByFlightIdQuery request, CancellationToken cancellationToken)
        {
            var flightDto = await _unitOfWork.FlightRepository.GetByCondition()
                .AsNoTracking()
                .ProjectToType<FlightDetailDto>()
                .FirstOrDefaultAsync(f => f.FlightId == request.FlightId, cancellationToken);

            if (flightDto == null)
                return ApiResult<FlightDetailDto>.Failure(["Không tìm thấy chuyến bay"]);

            return ApiResult<FlightDetailDto>.Success(flightDto);
        }
    }
}