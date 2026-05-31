using Application.Common;
using Application.CQRS.Payments.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Application.Services;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Payments.Commands.ProcessPaymentCallback
{
    //public class ProcessPaymentCallbackHandler : IRequestHandler<ProcessPaymentCallbackCommand, ApiResult<ProcessCallbackDto>>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IEnumerable<IPaymentGateway> _gateways;
    //    private readonly IEmailService _emailService;

    //    public ProcessPaymentCallbackHandler(IUnitOfWork unitOfWork, IEnumerable<IPaymentGateway> gateways, IEmailService emailService)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _gateways = gateways;
    //        _emailService = emailService;
    //    }

    //    public async Task<ApiResult<ProcessCallbackDto>> Handle(ProcessPaymentCallbackCommand request, CancellationToken cancellationToken)
    //    {
    //        var gateway = _gateways.FirstOrDefault(g =>
    //            g.Method.ToString().Equals(request.Method, StringComparison.OrdinalIgnoreCase));

    //        if (gateway is null)
    //            return ApiResult<ProcessCallbackDto>.Failure("Phương thức thanh toán không hợp lệ");

    //        var callbackResult = await gateway.ProcessCallbackAsync(request.Parameters);

    //        var booking = await _unitOfWork.BookingRepository
    //            .GetByCondition(b => b.BookingCode == callbackResult.BookingCode)
    //            .FirstOrDefaultAsync(cancellationToken);

    //        if (booking is null)
    //            return ApiResult<ProcessCallbackDto>.Failure("Không tìm thấy booking");

    //        var payment = await _unitOfWork.PaymentRepository
    //            .GetByCondition(
    //                expression: p => p.BookingId == booking.BookingId && p.Status == PaymentStatus.Pending,
    //                order: q => q.OrderByDescending(p => p.CreatedAt))
    //            .FirstOrDefaultAsync(cancellationToken);

    //        if (payment is null)
    //            return ApiResult<ProcessCallbackDto>.Failure("Không tìm thấy thông tin thanh toán");

    //        if (callbackResult.IsSuccess)
    //        {
    //            payment.Status = PaymentStatus.Success;
    //            booking.Status = BookingStatus.Confirmed;
    //        }
    //        else
    //        {
    //            payment.Status = PaymentStatus.Failed;
    //            booking.Status = BookingStatus.Failed;
    //        }

    //        _unitOfWork.PaymentRepository.Update(payment);
    //        _unitOfWork.BookingRepository.Update(booking);
    //        await _unitOfWork.SaveChangesAsync();

    //        if (callbackResult.IsSuccess)
    //        {
    //            var user = await _unitOfWork.UserRepository
    //                .GetByIdAsync(booking.UserId);

    //            if (user is not null)
    //            {
    //                _ = _emailService.SendBookingConfirmationAsync(new BookingConfirmationEmailDto(
    //                    ToEmail: user.Email,
    //                    CustomerName: user.FullName,
    //                    BookingCode: booking.BookingCode,
    //                    TripType: booking.TripType,
    //                    PaymentMethod: payment.Method.ToString(),
    //                    TotalPrice: booking.TotalPrice,
    //                    BookingDate: booking.BookingDate
    //                ));
    //            }
    //        }

    //        var message = callbackResult.IsSuccess ? "Thanh toán thành công" : "Thanh toán thất bại";
    //        return ApiResult<ProcessCallbackDto>.Success(new ProcessCallbackDto
    //        {
    //            IsSuccess = callbackResult.IsSuccess,
    //            BookingId = booking.BookingId,
    //            Message = message
    //        });
    //    }
    //}
}