using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.CQRS.Airlines.Commands.Create
{
    public class CreateAirlineHandler : IRequestHandler<CreateAirlineCommand, ApiResult<AirlineDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateAirlineHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AirlineDto>> Handle(CreateAirlineCommand request, CancellationToken cancellationToken)
        {
            var airline = request.Adapt<Airline>();
            await _unitOfWork.AirlineRepository.AddAsync(airline);
            await _unitOfWork.SaveChangesAsync();
            var airlineDto = airline.Adapt<AirlineDto>();
            return ApiResult<AirlineDto>.Success(airlineDto);
        }
    }
}
