using Application.Common;
using Application.CQRS.SeatReserve.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SeatReserve.Queries.GetSeatMap
{
    public class GetSeatMapHandler : IRequestHandler<GetSeatMapQuery, ApiResult<SeatMapDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSeatMapHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<SeatMapDto>> Handle(GetSeatMapQuery request, CancellationToken cancellationToken)
        {
            var isBookingValid = await _unitOfWork.BookingDetailRepository
                .GetByCondition(bd => bd.BookingId == request.BookingId && bd.BookingFlightId == request.FlightId)
                .AsNoTracking()
                .AnyAsync(cancellationToken);

            if (!isBookingValid)
                return ApiResult<SeatMapDto>.Failure("Chuyến bay không thuộc đơn đặt vé này");

            var seatTemplates = await _unitOfWork.SeatTemplateRepository
                .GetByCondition()
                .Include(st => st.SeatClass)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var flightSeats = await _unitOfWork.FlightSeatRepository
                .GetByCondition(fs => fs.FlightId == request.FlightId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var flightSeatBySeatId = flightSeats.ToDictionary(fs => fs.SeatId);
            var now = DateTime.UtcNow;

            var classGroups = seatTemplates
                .GroupBy(st => st.ClassId)
                .OrderByDescending(g => g.Key)
                .Select(classGroup => new SeatClassGroupDto
                {
                    ClassId = classGroup.Key,
                    ClassName = classGroup.First().SeatClass.ClassName,
                    Rows = classGroup
                        .GroupBy(st => st.RowIndex)
                        .OrderBy(r => r.Key)
                        .Select(rowGroup => new SeatRowDto
                        {
                            RowIndex = rowGroup.Key,
                            Seats = rowGroup
                                .OrderBy(st => st.ColIndex)
                                .Select(st => BuildSeatCellDto(st, flightSeatBySeatId, now))
                                .ToList()
                        }).ToList()
                }).ToList();

            return ApiResult<SeatMapDto>.Success(new SeatMapDto
            {
                FlightId = request.FlightId,
                ClassGroups = classGroups
            });
        }

        private static SeatCellDto BuildSeatCellDto(SeatTemplate st, IReadOnlyDictionary<int, FlightSeat> flightSeatBySeatId, DateTime now)
        {
            flightSeatBySeatId.TryGetValue(st.SeatId, out var fs);

            SeatStatus displayStatus = SeatStatus.Available;
            int? lockedByPassengerId = null;
            int flightSeatId = fs?.FlightSeatId ?? 0;

            if (fs is not null)
            {
                if (fs.Status == SeatStatus.Booked)
                {
                    displayStatus = SeatStatus.Booked;
                }
                else if (fs.Status == SeatStatus.Locked && fs.LockedUntil > now)
                {
                    displayStatus = SeatStatus.Locked;
                    lockedByPassengerId = fs.LockedBy;
                }
            }

            return new SeatCellDto
            {
                FlightSeatId = flightSeatId,
                SeatId = st.SeatId,
                SeatNumber = st.SeatNumber,
                ColIndex = st.ColIndex,
                Status = displayStatus,
                LockedByPassengerId = lockedByPassengerId
            };
        }
    }
}