using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Services.Queries.GetById
{
    public class GetByServiceIdHandler : IRequestHandler<GetByServiceIdQuery, ApiResult<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetByServiceIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<ServiceDto>> Handle(GetByServiceIdQuery request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.ServiceRepository.GetByCondition()
                 .AsNoTracking()
                 .FirstOrDefaultAsync(s => s.ServiceId == request.ServiceId, cancellationToken);
            if (service == null)
                return ApiResult<ServiceDto>.Failure(["Dịch vụ không tồn tại"]);

            var serviceDto = service.Adapt<ServiceDto>();
            return ApiResult<ServiceDto>.Success(serviceDto);
        }
    }
}
