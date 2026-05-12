using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.CQRS.Airlines.Queries.GetById
{
    public class GetByAirlineIdHandler : IRequestHandler<GetByAirlineIdQuery, ApiResult<AirlineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetByAirlineIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AirlineDto>> Handle(GetByAirlineIdQuery request, CancellationToken cancellationToken)
        {
            var airline = await _unitOfWork.AirlineRepository.GetByIdAsync(request.AirlineId);
            if (airline == null) 
                return ApiResult<AirlineDto>.Failure(["Hãng bay không tồn tại"]);

            var airlineDto = airline.Adapt<AirlineDto>();
            return ApiResult<AirlineDto>.Success(airlineDto);
        }
    }
}
