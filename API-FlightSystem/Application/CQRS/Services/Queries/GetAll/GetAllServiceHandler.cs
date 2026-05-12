using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;


namespace Application.CQRS.Services.Queries.GetAll
{
    public class GetAllServiceHandler : IRequestHandler<GetAllServiceQuery, ApiResult<PageList<ServiceDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllServiceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<ServiceDto>>> Handle(GetAllServiceQuery request, CancellationToken cancellationToken)
        {
            var service = _unitOfWork.ServiceRepository.GetByCondition();
            if (!string.IsNullOrEmpty(request.Search))
                service = service.Where(a => a.ServiceName.Contains(request.Search));

            if (request.IsActive.HasValue)
                service = service.Where(a => a.IsActive == request.IsActive.Value);

            service = service.OrderBy(a => a.ServiceId);

            var pagedList = await PageList<ServiceDto>.ToPagedListAsync(
                service.ProjectToType<ServiceDto>(),
                request.PageIndex,
                request.PageSize
            );

            return ApiResult<PageList<ServiceDto>>.Success(pagedList);
        }
    }
}
