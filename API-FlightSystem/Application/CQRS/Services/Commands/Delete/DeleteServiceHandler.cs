using Application.Common;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.CQRS.Services.Commands.Delete
{
    public class DeleteServiceHandler : IRequestHandler<DeleteServiceCommand, ApiResult<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteServiceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<ServiceDto>> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.ServiceRepository
                .GetByIdAsync(request.ServiceId);

            if (service is null)
                return ApiResult<ServiceDto>.Failure(["Dịch vụ không tồn tại"]);

            if (!service.IsActive)
                return ApiResult<ServiceDto>.Failure(["Dịch vụ này đã ngừng cung cấp trước đó"]);

            var isInactiveFlight = await _unitOfWork.FlightServiceRepository
                .GetByCondition(fs => fs.ServiceId == request.ServiceId
                                   && fs.Flight.Status == FlightStatus.Active)
                .AnyAsync();

            if (isInactiveFlight)
                return ApiResult<ServiceDto>.Failure(["Dịch vụ đang được sử dụng trong chuyến bay hoạt động, không thể ngừng cung cấp"]);

            service.IsActive = false;
            var serviceDto = service.Adapt<ServiceDto>();
            return ApiResult<ServiceDto>.Success(serviceDto);
        }
    }
}
