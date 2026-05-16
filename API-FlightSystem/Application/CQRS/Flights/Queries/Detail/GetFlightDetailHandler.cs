using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Flights.Queries.Detail
{
    public class GetFlightDetailHandler : IRequestHandler<GetFlightDetailQuery, ApiResult<FlightDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetFlightDetailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<FlightDetailDto>> Handle(GetFlightDetailQuery request, CancellationToken cancellationToken)
        {
            var flight = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.FlightId == request.FlightId
                                  && f.Status == FlightStatus.Active)
                .Include(f => f.Plane).ThenInclude(p => p.Airline)
                .Include(f => f.Route).ThenInclude(r => r.OriginAirport)
                .Include(f => f.Route).ThenInclude(r => r.DestinationAirport)
                .Include(f => f.Policy)
                .Include(f => f.FlightSegments.OrderBy(s => s.SegmentOrder)).ThenInclude(s => s.Route).ThenInclude(r => r.OriginAirport)
                .Include(f => f.FlightSegments).ThenInclude(s => s.Route).ThenInclude(r => r.DestinationAirport)
                .Include(f => f.FlightServices).ThenInclude(fs => fs.Service)
                .FirstOrDefaultAsync();

            if (flight == null)
                return ApiResult<FlightDetailDto>.Failure(["Chuyến bay không tồn tại."]);

            var flightDto = flight.Adapt<FlightDetailDto>();
            return ApiResult<FlightDetailDto>.Success(flightDto);
        }
    }
}
