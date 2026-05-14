using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Flights.Queries.GetById
{
    public class GetByFlightIdHandler : IRequestHandler<GetByFlightIdQuery, ApiResult<FlightDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByFlightIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<FlightDto>> Handle(GetByFlightIdQuery request, CancellationToken cancellationToken)
        {
            var flight = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.FlightId == request.FlightId)
                .Include(f => f.Policy)
                .Include(f => f.FlightSegments.OrderBy(s => s.SegmentOrder))
                .Include(f => f.FlightSeatPrices)
                .Include(f => f.FlightServices)
                .FirstOrDefaultAsync(cancellationToken);

            if (flight == null)
                return ApiResult<FlightDto>.Failure(["Chuyến bay không tồn tại."]);

            var flightDto = flight.Adapt<FlightDto>();
            return ApiResult<FlightDto>.Success(flightDto);
        }
    }
}
