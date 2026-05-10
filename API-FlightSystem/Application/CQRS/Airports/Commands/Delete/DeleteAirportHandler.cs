using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;

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
                return ApiResult<AirportDto>.Failure(new[] { "Sân bay không tồn tại" });

            if (airport.Status == FlightStatus.Inactive)
                return ApiResult<AirportDto>.Failure(new[] { "Sân bay đã bị vô hiệu hóa trước đó" });

            airport.Status = FlightStatus.Inactive;

            var airportDto = airport.Adapt<AirportDto>();
            return ApiResult<AirportDto>.Success(airportDto);

            /* Kiểm tra có Route nào đang Active dùng sân bay này không OriginAirportId hoặc DestinationAirportId
             * Kiểm tra có FlightSegment nào đang Active không
             * Nếu có → không cho vô hiệu hóa */
        }
    }
}