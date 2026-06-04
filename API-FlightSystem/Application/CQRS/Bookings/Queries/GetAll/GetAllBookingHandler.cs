using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.Bookings.Queries.GetAll;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Bookings.Queries.GetAllBooking
{
    public class GetAllBookingHandler : IRequestHandler<GetAllBookingQuery, ApiResult<PageList<BookingListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBookingHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<BookingListDto>>> Handle(GetAllBookingQuery request, CancellationToken cancellationToken)
        {
            var booking = _unitOfWork.BookingRepository.GetByCondition().AsNoTracking();

            if (request.TripType.HasValue)
                booking = booking.Where(b => b.TripType == request.TripType);
            if (request.ClassId.HasValue)
                booking = booking.Where(b => b.ClassId == request.ClassId);
            if (request.BookingDate.HasValue)
                booking = booking.Where(b => b.BookingDate.Date == request.BookingDate.Value.Date);

            if (request.Status.HasValue)
                booking = booking.Where(a => a.Status == request.Status.Value);

            booking = booking.OrderByDescending(b => b.BookingDate);

            var pagedList = await PageList<BookingListDto>.ToPagedListAsync(
                booking.ProjectToType<BookingListDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<BookingListDto>>.Success(pagedList);
        }
    }
}