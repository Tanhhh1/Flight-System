using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using Application.Hubs;
using Application.Interfaces.Hubs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SeatReverse.Queries.GetFlightSeat
{
    public class GetFlightSeatHandler : IRequestHandler<GetFlightSeatQuery, ApiResult<List<SeatLayoutDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<SeatHub, ISeatHubClient> _hubContext;
        private readonly IMapper _mapper;

        public GetFlightSeatHandler(IUnitOfWork unitOfWork, IHubContext<SeatHub, ISeatHubClient> hubContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task<ApiResult<List<SeatLayoutDto>>> Handle(GetFlightSeatQuery request, CancellationToken cancellationToken)
        {
            await UnlockExpiredSeatsAsync(request.FlightId, cancellationToken);
            var seats = await _unitOfWork.FlightSeatRepository
                .GetByCondition(s => s.FlightId == request.FlightId)
                .Include(s => s.SeatTemplate).ThenInclude(st => st.SeatClass)
                .ToListAsync(cancellationToken);
            var result = _mapper.Map<List<SeatLayoutDto>>(seats);
            return ApiResult<List<SeatLayoutDto>>.Success(result);
        }

        private async Task UnlockExpiredSeatsAsync(int flightId, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var expiredSeats = await _unitOfWork.FlightSeatRepository
                .GetByCondition(s => s.FlightId == flightId && s.Status == SeatStatus.Locked && s.LockedUntil <= now)
                .Include(s => s.SeatTemplate)
                .ToListAsync(cancellationToken);

            if (!expiredSeats.Any()) return;

            foreach (var seat in expiredSeats)
            {
                seat.Status = SeatStatus.Available;
                seat.LockedUntil = default;
                seat.LockedBy = 0;
                _unitOfWork.FlightSeatRepository.Update(seat);

                await _hubContext.Clients
                    .Group(SeatHub.GetGroupName(flightId))
                    .SeatUnlocked(seat.FlightSeatId, seat.SeatTemplate.SeatNumber);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
