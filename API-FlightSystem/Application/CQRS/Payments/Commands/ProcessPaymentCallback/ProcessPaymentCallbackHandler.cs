using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.Payments.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Application.Services;
using Domain.Enums;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Payments.Commands.ProcessPaymentCallback
{
    public class ProcessPaymentCallbackHandler : IRequestHandler<ProcessPaymentCallbackCommand, ApiResult<ProcessCallbackDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _gateway;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;

        public ProcessPaymentCallbackHandler(IUnitOfWork unitOfWork, IPaymentGateway gateway, IEmailService emailService, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _gateway = gateway;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<ApiResult<ProcessCallbackDto>> Handle(ProcessPaymentCallbackCommand request, CancellationToken cancellationToken)
        {
            var callbackResult = await _gateway.ProcessCallbackAsync(request.Parameters);

            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingCode == callbackResult.BookingCode)
                .Include(b => b.BookingDetails)
                .FirstOrDefaultAsync(cancellationToken);

            if (booking is null)
                return ApiResult<ProcessCallbackDto>.Failure("Không tìm thấy booking");

            var payment = await _unitOfWork.PaymentRepository
                .GetByCondition(
                    expression: p => p.BookingId == booking.BookingId && p.Status == PaymentStatus.Pending,
                    order: q => q.OrderByDescending(p => p.CreatedAt))
                .FirstOrDefaultAsync(cancellationToken);

            if (payment is null)
                return ApiResult<ProcessCallbackDto>.Failure("Không tìm thấy thông tin thanh toán");

            if (callbackResult.IsSuccess)
            {
                payment.Status = PaymentStatus.Success;
                booking.Status = BookingStatus.Confirmed;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                booking.Status = BookingStatus.Failed;
            }

            _unitOfWork.PaymentRepository.Update(payment);
            _unitOfWork.BookingRepository.Update(booking);

            if (callbackResult.IsSuccess)
            {
                var flightPassengerCounts = booking.BookingDetails
                    .GroupBy(bd => bd.BookingFlightId)
                    .Select(g => new { FlightId = g.Key, PassengerCount = g.Count() });

                foreach (var item in flightPassengerCounts)
                {
                    var seatPrice = await _unitOfWork.FlightSeatPriceRepository
                        .GetByCondition(p => p.FlightId == item.FlightId
                                         && p.ClassId == booking.ClassId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (seatPrice is not null)
                    {
                        seatPrice.AvailableSeats = Math.Max(0, seatPrice.AvailableSeats - item.PassengerCount);
                        _unitOfWork.FlightSeatPriceRepository.Update(seatPrice);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            if (callbackResult.IsSuccess)
            {
                var user = await _userManager.FindByIdAsync(booking.UserId.ToString());

                if (user is not null)
                {
                    var bookingDetail = await _unitOfWork.BookingRepository
                        .GetByCondition(b => b.BookingId == booking.BookingId)
                        .Include(b => b.User)
                        .Include(b => b.SeatClass)
                        .Include(b => b.BookingDetails).ThenInclude(bd => bd.Flight).ThenInclude(f => f.Plane).ThenInclude(p => p.Airline)
                        .Include(b => b.BookingDetails).ThenInclude(bd => bd.Flight).ThenInclude(f => f.Route).ThenInclude(r => r.OriginAirport)
                        .Include(b => b.BookingDetails).ThenInclude(bd => bd.Flight).ThenInclude(f => f.Route).ThenInclude(r => r.DestinationAirport)
                        .Include(b => b.BookingDetails).ThenInclude(bd => bd.Flight).ThenInclude(f => f.FlightSegments).ThenInclude(s => s.Route).ThenInclude(r => r.OriginAirport)
                        .Include(b => b.BookingDetails).ThenInclude(bd => bd.Flight).ThenInclude(f => f.FlightSegments).ThenInclude(s => s.Route).ThenInclude(r => r.DestinationAirport)
                        .Include(b => b.BookingDetails).ThenInclude(bd => bd.Passenger)
                        .FirstOrDefaultAsync(cancellationToken);

                    var bookingDto = bookingDetail!.Adapt<BookingByIdDto>();

                    var flights = bookingDto.Flights.Select(f => new FlightEmailDto(
                        OriginAirport: f.OriginAirport,
                        OriginAirportName: f.OriginAirportName,
                        DestinationAirport: f.DestinationAirport,
                        DestinationAirportName: f.DestinationAirportName,
                        DepartureTime: f.DepartureTime,
                        ArrivalTime: f.ArrivalTime,
                        AirlineName: f.AirlineName,
                        PlaneModel: f.PlaneModel,
                        Passengers: f.Passengers.Select(p => new PassengerEmailDto(
                            FullName: p.FullName,
                            Gender: p.Gender,
                            UnitPrice: p.UnitPrice
                        )).ToList()
                    )).ToList();

                    _ = _emailService.SendBookingConfirmationAsync(new BookingConfirmationEmailDto(
                        ToEmail: user.Email!,
                        CustomerName: user.Fullname,
                        BookingCode: booking.BookingCode,
                        TripType: booking.TripType switch
                        {
                            TripType.OneWay => "Một chiều",
                            TripType.RoundTrip => "Khứ hồi",
                            TripType.MultiCity => "Nhiều điểm đến",
                            _ => booking.TripType.ToString()
                        },
                        PaymentMethod: payment.Method switch
                        {
                            PaymentMethod.DomesticCard => "Thẻ nội địa",
                            PaymentMethod.EWallet => "Ví điện tử",
                            PaymentMethod.InternationalCard => "Thẻ quốc tế",
                            _ => payment.Method.ToString()
                        },
                        TotalPrice: booking.TotalPrice,
                        BookingDate: booking.BookingDate,
                        Flights: flights
                    ));
                }
            }

            var message = callbackResult.IsSuccess ? "Thanh toán thành công" : "Thanh toán thất bại";
            return ApiResult<ProcessCallbackDto>.Success(new ProcessCallbackDto
            {
                IsSuccess = callbackResult.IsSuccess,
                BookingId = booking.BookingId,
                Message = message
            });
        }
    }
}