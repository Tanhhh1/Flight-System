using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums; // Thêm Enums để dùng FlightStatus
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore; // Thêm để dùng .AnyAsync()

namespace Application.CQRS.Airports.Commands.Update
{
    public class UpdateAirportHandler : IRequestHandler<UpdateAirportCommand, ApiResult<AirportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAirportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AirportDto>> Handle(UpdateAirportCommand request, CancellationToken cancellationToken)
        {
            var airport = await _unitOfWork.AirportRepository.GetByIdAsync(request.AirportId);
            if (airport == null)
                return ApiResult<AirportDto>.Failure("Sân bay không tồn tại");

            var unfinishedFlight = await _unitOfWork.FlightRepository
                .GetByCondition(f => (f.Route.OriginAirportId == request.AirportId || f.Route.DestinationAirportId == request.AirportId)
                                  && (f.Status == FlightStatus.Active || f.Status == FlightStatus.Delayed))
                .AnyAsync(cancellationToken);
            if (unfinishedFlight)
                return ApiResult<AirportDto>.Failure("Không thể cập nhật thông tin vì sân bay đang có chuyến bay chưa hoàn thành");

            request.Adapt(airport);
            _unitOfWork.AirportRepository.Update(airport);

            var airportDto = airport.Adapt<AirportDto>();
            return ApiResult<AirportDto>.Success(airportDto);
        }
    }
}