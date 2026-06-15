using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var existingAirline = await _unitOfWork.AirlineRepository
                .GetByCondition(a => a.AirlineCode == request.AirlineCode)
                .AnyAsync(cancellationToken);

            if (existingAirline != null)
                return ApiResult<AirlineDto>.Failure($"Mã hãng hàng không '{request.AirlineCode}' đã tồn tại.");

            var airline = request.Adapt<Airline>();
            airline.Status = FlightStatus.Active;

            await _unitOfWork.AirlineRepository.AddAsync(airline);
            await _unitOfWork.SaveChangesAsync();

            var airlineDto = airline.Adapt<AirlineDto>();
            return ApiResult<AirlineDto>.Success(airlineDto);
        }
    }
}
