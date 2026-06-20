using Application.Common;
using Application.CQRS.Support.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Support.Queries.GetAll
{
    public class GetAllSupportRequestsHandler : IRequestHandler<GetAllSupportRequestsQuery, ApiResult<PageList<SupportRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSupportRequestsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<SupportRequestDto>>> Handle(GetAllSupportRequestsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.SupportRequestRepository.GetByCondition();

            if (request.RequestType.HasValue)
                query = query.Where(sr => sr.RequestType == request.RequestType);

            if (request.Status.HasValue)
                query = query.Where(sr => sr.Status == request.Status);

            query = query.OrderByDescending(sr => sr.CreatedAt);

            var pagedList = await PageList<SupportRequestDto>.ToPagedListAsync(
                query.AsNoTracking().ProjectToType<SupportRequestDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<SupportRequestDto>>.Success(pagedList);
        }
    }
}
