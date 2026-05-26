using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Airports.Commands.Delete
{
    public class DeleteAirportHandler : IRequestHandler<DeleteAirportCommand, ApiResult<AirportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAirportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AirportDto>> Handle(DeleteAirportCommand request, CancellationToken cancellationToken)
        {
            var airport = await _unitOfWork.AirportRepository.GetByIdAsync(request.AirportId);

            if (airport == null)
                return ApiResult<AirportDto>.Failure("Sân bay không tồn tại");

            if (airport.Status == FlightStatus.Inactive)
                return ApiResult<AirportDto>.Failure("Sân bay đã bị vô hiệu hóa trước đó");

            bool hasActiveRoute = await _unitOfWork.RouteRepository
                .GetByCondition(r => r.Status == FlightStatus.Active
                                  && (r.OriginAirportId == request.AirportId
                                   || r.DestinationAirportId == request.AirportId))
                .AnyAsync(cancellationToken);
            if (hasActiveRoute)
                return ApiResult<AirportDto>.Failure("Sân bay đang được sử dụng trong tuyến bay hoạt động");

            airport.Status = FlightStatus.Inactive;

            var airportDto = airport.Adapt<AirportDto>();
            return ApiResult<AirportDto>.Success(airportDto);
        }
    }
}