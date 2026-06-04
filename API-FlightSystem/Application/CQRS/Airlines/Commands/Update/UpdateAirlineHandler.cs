using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Airlines.Commands.Update
{
    public class UpdateAirlineHandler : IRequestHandler<UpdateAirlineCommand, ApiResult<AirlineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateAirlineHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AirlineDto>> Handle(UpdateAirlineCommand request, CancellationToken cancellationToken)
        {
            var airline = await _unitOfWork.AirlineRepository.GetByIdAsync(request.AirlineId);
            if (airline == null)
                return ApiResult<AirlineDto>.Failure("Hãng bay không tồn tại");

            bool unfinishedFlightFlight = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.Plane.AirlineId == request.AirlineId
                                  && (f.Status == FlightStatus.Active || f.Status == FlightStatus.Delayed))
                .AnyAsync(cancellationToken);
            if (unfinishedFlightFlight)
                return ApiResult<AirlineDto>.Failure("Không thể cập nhật thông tin vì hãng bay đang có chuyến bay chưa hoàn thành");

            request.Adapt(airline);
            _unitOfWork.AirlineRepository.Update(airline);

            var airlineDto = airline.Adapt<AirlineDto>();
            return ApiResult<AirlineDto>.Success(airlineDto);
        }
    }
}