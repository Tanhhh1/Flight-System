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
            var query = _unitOfWork.FlightRepository
                .GetByCondition()
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.OriginCity))
                query = query.Where(f =>
                    f.Route.OriginAirport.City.Contains(request.OriginCity) ||
                    f.Route.OriginAirport.AirportCode.Contains(request.OriginCity));

            if (!string.IsNullOrWhiteSpace(request.DestinationCity))
                query = query.Where(f =>
                    f.Route.DestinationAirport.City.Contains(request.DestinationCity) ||
                    f.Route.DestinationAirport.AirportCode.Contains(request.DestinationCity));

            if (request.DepartureDate.HasValue)
                query = query.Where(f => f.DepartureTime.Date == DateTime.SpecifyKind(request.DepartureDate.Value.Date, DateTimeKind.Utc));

            if (request.Status.HasValue)
                query = query.Where(f => f.Status == request.Status.Value);

            if (request.AirlineId.HasValue)
                query = query.Where(f => f.Plane.AirlineId == request.AirlineId.Value);

            var result = await PageList<FlightListDto>.ToPagedListAsync(
                query.OrderByDescending(f => f.DepartureTime).ProjectToType<FlightListDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken);

            return ApiResult<PageList<FlightListDto>>.Success(result);
        }
    }
}
