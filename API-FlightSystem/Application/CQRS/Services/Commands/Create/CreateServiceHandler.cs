using Application.Common;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.CQRS.Services.Commands.Create
{
    public class CreateServiceHandler : IRequestHandler<CreateServiceCommand, ApiResult<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateServiceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<ServiceDto>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = request.Adapt<Service>();
            service.IsActive = true;
            await _unitOfWork.ServiceRepository.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();
            var result = service.Adapt<ServiceDto>();
            return ApiResult<ServiceDto>.Success(result);
        }
    }
}
