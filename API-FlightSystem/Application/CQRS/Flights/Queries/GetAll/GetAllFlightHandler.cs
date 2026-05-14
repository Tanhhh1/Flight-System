using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.UnitOfWork;
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
            var flights = _unitOfWork.FlightRepository
                .GetByCondition()
                .Include(f => f.Plane).ThenInclude(p => p.Airline)
                .Include(f => f.Route).ThenInclude(r => r.OriginAirport)
                .Include(f => f.Route).ThenInclude(r => r.DestinationAirport)
                .Include(f => f.Policy)
                .Include(f => f.FlightSeatPrices)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
                flights = flights.Where(f =>
                    f.Plane.Airline.AirlineName.Contains(request.Search) ||
                    f.Plane.PlaneModel.Contains(request.Search));

            if (request.RouteId.HasValue)
                flights = flights.Where(f => f.RouteId == request.RouteId.Value);

            if (request.AirlineId.HasValue)
                flights = flights.Where(f => f.Plane.AirlineId == request.AirlineId.Value);

            if (request.PlaneId.HasValue)
                flights = flights.Where(f => f.PlaneId == request.PlaneId.Value);

            if (request.Status.HasValue)
                flights = flights.Where(f => f.Status == request.Status.Value);

            if (request.DepartureFrom.HasValue)
                flights = flights.Where(f => f.DepartureTime >= request.DepartureFrom.Value);
            if (request.DepartureTo.HasValue)
                flights = flights.Where(f => f.DepartureTime <= request.DepartureTo.Value);

            if (request.MinPrice.HasValue)
                flights = flights.Where(f =>
                    f.FlightSeatPrices.Any(p => p.Price >= request.MinPrice.Value));
            if (request.MaxPrice.HasValue)
                flights = flights.Where(f =>
                    f.FlightSeatPrices.Any(p => p.Price <= request.MaxPrice.Value));

            if (request.IsRefund.HasValue)
                flights = flights.Where(f => f.Policy.IsRefund == request.IsRefund.Value);
            if (request.IsChange.HasValue)
                flights = flights.Where(f => f.Policy.IsChange == request.IsChange.Value);

            flights = flights.OrderByDescending(f => f.DepartureTime);

            var pagedList = await PageList<FlightListDto>.ToPagedListAsync(
                flights.ProjectToType<FlightListDto>(),
                request.PageIndex,
                request.PageSize
            );

            return ApiResult<PageList<FlightListDto>>.Success(pagedList);
        }
    }
}
