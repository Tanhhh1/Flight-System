using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Airlines.Commands.Delete
{
    public class DeleteAirlineHandler : IRequestHandler<DeleteAirlineCommand, ApiResult<AirlineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteAirlineHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AirlineDto>> Handle(DeleteAirlineCommand request, CancellationToken cancellationToken)
        {
            var airline = await _unitOfWork.AirlineRepository.GetByIdAsync(request.AirlineId);
            if (airline == null)
                return ApiResult<AirlineDto>.Failure(["Hãng bay không tồn tại"]);

            if (airline.Status == FlightStatus.Inactive)
                return ApiResult<AirlineDto>.Failure(["Hãng bay đã bị vô hiệu hóa trước đó"]);

            bool hasActivePlane = await _unitOfWork.PlaneRepository
                .GetByCondition(p => p.AirlineId == request.AirlineId
                                  && p.Status == FlightStatus.Active)
                .AnyAsync();
            if (hasActivePlane)
                return ApiResult<AirlineDto>.Failure(["Hãng bay đang có máy bay hoạt động"]);

            airline.Status = FlightStatus.Inactive;

            var airlineDto = airline.Adapt<AirlineDto>();
            return ApiResult<AirlineDto>.Success(airlineDto);
        }
    }
}
