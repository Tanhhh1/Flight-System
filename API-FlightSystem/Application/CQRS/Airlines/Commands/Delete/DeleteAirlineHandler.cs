using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MediatR;

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
                return ApiResult<AirlineDto>.Failure(new[] { "Hãng bay không tồn tại" });

            if (airline.Status == FlightStatus.Inactive)
                return ApiResult<AirlineDto>.Failure(new[] { "Hãng bay đã bị vô hiệu hóa trước đó" });

            airline.Status = FlightStatus.Inactive;

            var airlineDto = airline.Adapt<AirlineDto>();
            return ApiResult<AirlineDto>.Success(airlineDto);

            /* Kiểm tra có Plane nào đang Active không
             * Nếu có Plane Active → không cho vô hiệu hóa
             * Hoặc tự động Inactive tất cả Plane → tùy nghiệp vụ */

            /* Nếu Plane bị Inactive → kiểm tra Flight liên quan
             * Flight đang Active/Delayed → tự động Cancelled
             * Flight Completed → giữ nguyên */

            /* Booking liên quan đến Flight bị Cancelled → cần xử lý */
        }
    }
}
