using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using MediatR;

namespace Application.CQRS.SeatReverse.Queries.GetBookingPassengers
{
    public class GetBookingPassengersQuery : IRequest<ApiResult<BookingPassengersDto>>
    {
        public string BookingCode { get; set; } = null!;
    }
}
