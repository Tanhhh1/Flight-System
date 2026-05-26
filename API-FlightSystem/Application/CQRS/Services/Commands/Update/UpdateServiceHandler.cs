using Application.Common;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.CQRS.Services.Commands.Update
{
    public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, ApiResult<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateServiceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<ServiceDto>> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
                return ApiResult<ServiceDto>.Failure("Dịch vụ không tồn tại");
            request.Adapt(service);
            _unitOfWork.ServiceRepository.Update(service);
            var result = service.Adapt<ServiceDto>();
            return ApiResult<ServiceDto>.Success(result);
        }
    }
}
