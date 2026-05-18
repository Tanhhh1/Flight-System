using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.Bookings.Queries.GetAll;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Bookings.Queries.GetAllBooking
{
    public class GetAllBookingHandler : IRequestHandler<GetAllBookingQuery, ApiResult<PageList<BookingDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBookingHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<BookingDto>>> Handle(GetAllBookingQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.BookingRepository
                .GetByCondition()
                .Include(b => b.User)
                .Include(b => b.SeatClass)
                .Include(b => b.BookingDetails).ThenInclude(d => d.Passenger)
                .Include(b => b.BookingDetails).ThenInclude(d => d.Flight)
                .AsNoTracking();

            if (request.TripType.HasValue)
                query = query.Where(b => b.TripType == (int)request.TripType);

            if (request.ClassId.HasValue)
                query = query.Where(b => b.ClassId == request.ClassId);

            if (request.BookingDate.HasValue)
                query = query.Where(b => b.BookingDate.Date == request.BookingDate.Value.Date);

            query = query.OrderByDescending(b => b.BookingDate);

            var pagedList = await PageList<BookingDto>.ToPagedListAsync(
                query.ProjectToType<BookingDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<BookingDto>>.Success(pagedList);
        }
    }
}