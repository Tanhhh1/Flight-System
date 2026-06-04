using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Flights.Queries.GetAll
{
    public class GetAllFlightHandler : IRequestHandler<GetAllFlightQuery, ApiResult<PageList<FlightListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllFlightHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<FlightListDto>>> Handle(GetAllFlightQuery request, CancellationToken cancellationToken)
        {
            var flight = _unitOfWork.FlightRepository
                .GetByCondition()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.OriginAirportCode))
                flight = flight.Where(f => f.Route.OriginAirport.AirportCode == request.OriginAirportCode);

            if (!string.IsNullOrWhiteSpace(request.DestinationAirportCode))
                flight = flight.Where(f => f.Route.DestinationAirport.AirportCode == request.DestinationAirportCode);

            if (request.DepartureDate.HasValue)
                flight = flight.Where(f => f.DepartureTime.Date == request.DepartureDate.Value.Date);

            if (request.Status.HasValue)
                flight = flight.Where(f => f.Status == request.Status.Value);

            if (request.AirlineId.HasValue)
                flight = flight.Where(f => f.Plane.AirlineId == request.AirlineId.Value);

            var result = await PageList<FlightListDto>.ToPagedListAsync(
                flight.OrderByDescending(f => f.DepartureTime).ProjectToType<FlightListDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken);

            return ApiResult<PageList<FlightListDto>>.Success(result);
        }
    }
}
