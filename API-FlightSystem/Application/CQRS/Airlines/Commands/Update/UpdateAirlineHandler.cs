using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

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
            if(airline == null)
                return ApiResult<AirlineDto>.Failure(new[] { "Hãng bay không tồn tại" });
            request.Adapt(airline);
            _unitOfWork.AirlineRepository.Update(airline);
            var airlineDto = request.Adapt<AirlineDto>();
            return ApiResult<AirlineDto>.Success(airlineDto);
        }
    }
}
