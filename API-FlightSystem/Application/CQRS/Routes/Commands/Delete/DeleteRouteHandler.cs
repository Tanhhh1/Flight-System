using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.CQRS.Routes.Commands.Delete
{
    public class DeleteRouteHandler : IRequestHandler<DeleteRouteCommand, ApiResult<RouteDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteRouteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<RouteDto>> Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await _unitOfWork.RouteRepository.GetByIdAsync(request.RouteId);
            if (route == null)
                return ApiResult<RouteDto>.Failure(new[] { "Tuyến bay không tồn tại" });

            if (route.Status == FlightStatus.Inactive)
                return ApiResult<RouteDto>.Failure(new[] { "Tuyến bay đã ngừng hoạt động" });

            route.Status = FlightStatus.Inactive;

            var routeDto = route.Adapt<RouteDto>();
            return ApiResult<RouteDto>.Success(routeDto);
        }

        /* Kiểm tra có Flight nào đang Active/Delayed không
         * Nếu có → không cho vô hiệu hóa
         * Hoặc tự động Cancelled → tùy nghiệp vụ */
    }
}
