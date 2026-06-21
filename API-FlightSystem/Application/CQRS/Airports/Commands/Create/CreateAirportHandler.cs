using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Airports.Commands.Create
{
    public class CreateAirportHandler : IRequestHandler<CreateAirportCommand, ApiResult<AirportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
    
        public CreateAirportHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<AirportDto>> Handle(CreateAirportCommand request, CancellationToken cancellationToken)
        {
            var existingAirport = await _unitOfWork.AirportRepository
                .GetByCondition(a => a.AirportCode == request.AirportCode)
                .AnyAsync(cancellationToken);

            if (existingAirport)
                return ApiResult<AirportDto>.Failure($"Mã sân bay '{request.AirportCode}' đã tồn tại.");
            // Adapt - map sau khi load data về memory
            var airport = request.Adapt<Airport>(); // chuyển từ command -> entity
            airport.Status = FlightStatus.Active;
            await _unitOfWork.AirportRepository.AddAsync(airport);
            await _unitOfWork.SaveChangesAsync();
            var airportDto = airport.Adapt<AirportDto>(); // chuyển từ entity -> Dto
            return ApiResult<AirportDto>.Success(airportDto);
        }
    }
}
