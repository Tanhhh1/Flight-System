using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SeatReverse.Queries.GetSeatMap
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
            var bookingDetails = await _unitOfWork.BookingDetailRepository
                .GetByCondition(bd => bd.BookingId == request.BookingId && bd.BookingFlightId == request.FlightId)
                .ToListAsync(cancellationToken);

            if (!bookingDetails.Any())
                return ApiResult<SeatMapDto>.Failure("Chuyến bay không thuộc đơn đặt vé này");

            var seatTemplates = await _unitOfWork.SeatTemplateRepository
                .GetByCondition()
                .Include(st => st.SeatClass)
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var flightSeats = await _unitOfWork.FlightSeatRepository
                .GetByCondition(fs => fs.FlightId == request.FlightId)
                .Include(fs => fs.BookingDetail)
                .ToListAsync(cancellationToken);

            var flightSeatBySeatId = flightSeats.ToDictionary(fs => fs.SeatId);

            var bookingDetailIds = bookingDetails.Select(bd => bd.BookingDetailId).ToHashSet();
            var seatToPassengerMap = bookingDetails
                .Where(bd => bd.FlightSeatId.HasValue)
                .ToDictionary(bd => bd.FlightSeatId!.Value, bd => bd.PassengerId);

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
                                .Select(st =>
                                {
                                    flightSeatBySeatId.TryGetValue(st.SeatId, out var fs);

                                    SeatStatus displayStatus;
                                    int? lockedByPassengerId = null;
                                    int flightSeatId = fs?.FlightSeatId ?? 0;

                                    if (fs == null)
                                    {
                                        displayStatus = SeatStatus.Available;
                                    }
                                    else if (fs.Status == SeatStatus.Booked)
                                    {
                                        displayStatus = SeatStatus.Booked;
                                    }
                                    else if (fs.Status == SeatStatus.Locked && fs.LockedUntil > now)
                                    {
                                        displayStatus = SeatStatus.Locked;
                                        if (seatToPassengerMap.ContainsKey(fs.FlightSeatId))
                                            lockedByPassengerId = seatToPassengerMap[fs.FlightSeatId];
                                    }
                                    else
                                    {
                                        displayStatus = SeatStatus.Available;
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
                                }).ToList()
                        }).ToList()
                }).ToList();

            return ApiResult<SeatMapDto>.Success(new SeatMapDto
            {
                FlightId = request.FlightId,
                ClassGroups = classGroups
            });
        }
    }
}
