using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Flights.Queries.Search
{
    public class SearchFlightHandler : IRequestHandler<SearchFlightQuery, ApiResult<PageList<FlightSearchDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchFlightHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<FlightSearchDto>>> Handle(SearchFlightQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.FlightRepository
                .GetByCondition()
                .Where(f => f.Status == FlightStatus.Active)
                .AsNoTracking();

            query = query.Where(f =>
                f.Route.OriginAirport.City.Contains(request.OriginCity) ||
                f.Route.OriginAirport.AirportCode.Contains(request.OriginCity));

            query = query.Where(f =>
                f.Route.DestinationAirport.City.Contains(request.DestinationCity) ||
                f.Route.DestinationAirport.AirportCode.Contains(request.DestinationCity));

            query = query.Where(f =>
                f.DepartureTime.Date == DateTime.SpecifyKind(request.DepartureDate.Date, DateTimeKind.Utc));

            query = query.Where(f =>
                f.FlightSeatPrices.Any(sp => sp.ClassId == request.ClassId && sp.AvailableSeats > 0));

            query = query.Where(f => f.FlightSeatPrices.Any(sp => sp.ClassId == request.ClassId));
             
            if (request.StopCount.HasValue)
                query = query.Where(f => f.FlightSegments.Count - 1 == request.StopCount.Value);

            if (request.AirlineId.HasValue)
                query = query.Where(f => f.Plane.AirlineId == request.AirlineId.Value);

            if (request.ServiceIds != null && request.ServiceIds.Any())
                query = query.Where(f => request.ServiceIds
                    .All(sid => f.FlightServices.Any(fs => fs.ServiceId == sid)));

            if (request.DepartureFromHour.HasValue)
                query = query.Where(f => f.DepartureTime.Hour >= request.DepartureFromHour.Value);

            if (request.DepartureToHour.HasValue)
                query = query.Where(f => f.DepartureTime.Hour <= request.DepartureToHour.Value);

            if (request.ArrivalFromHour.HasValue)
                query = query.Where(f => f.ArrivalTime.Hour >= request.ArrivalFromHour.Value);

            if (request.ArrivalToHour.HasValue)
                query = query.Where(f => f.ArrivalTime.Hour <= request.ArrivalToHour.Value);

            var result = await PageList<FlightSearchDto>.ToPagedListAsync(
                query.OrderBy(f => f.DepartureTime).ProjectToType<FlightSearchDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            foreach (var flight in result.Items)
            {
                flight.SeatClasses = flight.SeatClasses
                    .Where(sc => sc.ClassId == request.ClassId)
                    .ToList();
            }
            return ApiResult<PageList<FlightSearchDto>>.Success(result);
        }
    }
}
