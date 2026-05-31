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
            var query = _unitOfWork.BookingRepository.GetByCondition().AsNoTracking();

            if (request.TripType.HasValue)
                query = query.Where(b => b.TripType == (int)request.TripType);
            if (request.ClassId.HasValue)
                query = query.Where(b => b.ClassId == request.ClassId);
            if (request.BookingDate.HasValue)
                query = query.Where(b => b.BookingDate.Date == request.BookingDate.Value.Date);

            if (request.Status.HasValue)
                query = query.Where(a => a.Status == request.Status.Value);

            query = query.OrderByDescending(b => b.BookingDate);

            var pagedList = await PageList<BookingListDto>.ToPagedListAsync(
                query.ProjectToType<BookingListDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<BookingListDto>>.Success(pagedList);
        }
    }
}