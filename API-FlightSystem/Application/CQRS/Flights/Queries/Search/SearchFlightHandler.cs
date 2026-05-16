using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
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
                .GetByCondition(f => f.Status == FlightStatus.Active)
                .Include(f => f.Plane).ThenInclude(p => p.Airline)
                .Include(f => f.Route).ThenInclude(r => r.OriginAirport)
                .Include(f => f.Route).ThenInclude(r => r.DestinationAirport)
                .Include(f => f.Policy)
                .Include(f => f.FlightSegments)
                .Include(f => f.FlightSeatPrices).ThenInclude(sp => sp.SeatClass)
                .Include(f => f.FlightServices)
                .AsQueryable();

            if (request.OriginAirportId.HasValue)
                query = query.Where(f => f.Route.OriginAirportId == request.OriginAirportId.Value);

            if (request.DestinationAirportId.HasValue)
                query = query.Where(f => f.Route.DestinationAirportId == request.DestinationAirportId.Value);

            if (request.DepartureDate.HasValue)
                query = query.Where(f => f.DepartureTime.Date == request.DepartureDate.Value.Date);

            query = query.Where(f => f.FlightSeatPrices
                .Any(sp => sp.ClassId == request.ClassId && sp.AvailableSeats > 0));

            if (request.StopCount.HasValue)
                query = query.Where(f => f.FlightSegments.Count == request.StopCount.Value);

            if (request.AirlineId.HasValue)
                query = query.Where(f => f.Plane.AirlineId == request.AirlineId.Value);

            if (request.DepartureFromHour.HasValue)
                query = query.Where(f => f.DepartureTime.Hour >= request.DepartureFromHour.Value);
            if (request.DepartureToHour.HasValue)
                query = query.Where(f => f.DepartureTime.Hour <= request.DepartureToHour.Value);

            if (request.ArrivalFromHour.HasValue)
                query = query.Where(f => f.ArrivalTime.Hour >= request.ArrivalFromHour.Value);
            if (request.ArrivalToHour.HasValue)
                query = query.Where(f => f.ArrivalTime.Hour <= request.ArrivalToHour.Value);

            if (request.MinDuration.HasValue)
                query = query.Where(f => f.Route.FlightDuration >= request.MinDuration.Value);
            if (request.MaxDuration.HasValue)
                query = query.Where(f => f.Route.FlightDuration <= request.MaxDuration.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(f => f.FlightSeatPrices
                    .Any(sp => sp.ClassId == request.ClassId && sp.Price >= request.MinPrice.Value));
            if (request.MaxPrice.HasValue)
                query = query.Where(f => f.FlightSeatPrices
                    .Any(sp => sp.ClassId == request.ClassId && sp.Price <= request.MaxPrice.Value));

            if (request.ServiceIds.Count > 0)
                query = query.Where(f => request.ServiceIds
                    .All(sid => f.FlightServices.Any(fs => fs.ServiceId == sid)));

            if (request.IsRefund.HasValue)
                query = query.Where(f => f.Policy.IsRefund == request.IsRefund.Value);
            if (request.IsChange.HasValue)
                query = query.Where(f => f.Policy.IsChange == request.IsChange.Value);

            query = query.OrderByDescending(f => f.DepartureTime);

            var pagedFlights = await PageList<Flight>.ToPagedListAsync(
                query,
                request.PageIndex,
                request.PageSize);

            var dtos = pagedFlights.Items.Select(f =>
            {
                var dto = f.Adapt<FlightSearchDto>();

                var seatPrice = f.FlightSeatPrices
                    .FirstOrDefault(sp => sp.ClassId == request.ClassId);

                dto.Price = seatPrice?.Price ?? 0;
                dto.ClassId = request.ClassId;
                dto.ClassName = seatPrice?.SeatClass?.ClassName ?? string.Empty;

                return dto;
            }).ToList();

            var result = new PageList<FlightSearchDto>(
                dtos,
                pagedFlights.TotalCount,
                pagedFlights.PageIndex,
                pagedFlights.PageSize);

            return ApiResult<PageList<FlightSearchDto>>.Success(result);
        }
    }
}
