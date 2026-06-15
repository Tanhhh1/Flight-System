using Application.Common;
using Application.CQRS.Payments.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Payments.Commands.RetryPayment
{
    public class RetryPaymentHandler : IRequestHandler<RetryPaymentCommand, ApiResult<InitiateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _gateway;
        private const int MaxRetryCount = 3;

        public RetryPaymentHandler(IUnitOfWork unitOfWork, IPaymentGateway gateway)
        {
            _unitOfWork = unitOfWork;
            _gateway = gateway;
        }

        public async Task<ApiResult<InitiateDto>> Handle(RetryPaymentCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.BookingRepository
                .GetByIdAsync(request.BookingId);

            if (booking is null)
                return ApiResult<InitiateDto>.Failure("Không tìm thấy booking");

            if (booking.Status != BookingStatus.Failed)
                return ApiResult<InitiateDto>.Failure("Booking không đủ điều kiện để thanh toán lại");

            var failedCount = await _unitOfWork.PaymentRepository
                .GetByCondition(p => p.BookingId == request.BookingId && p.Status == PaymentStatus.Failed)
                .CountAsync(cancellationToken);

            if (failedCount >= MaxRetryCount)
                return ApiResult<InitiateDto>.Failure($"Đã vượt quá số lần thanh toán tối đa ({MaxRetryCount} lần)");

            var result = await _gateway.CreatePaymentUrlAsync(new PaymentRequest(
                BookingId: booking.BookingId,
                BookingCode: booking.BookingCode,
                Amount: booking.TotalPrice,
                Description: $"Thanh toan lai booking {booking.BookingCode}",
                ReturnUrl: request.ReturnUrl,
                IpAddress: request.ClientIp,
                Method: request.Method
            ));

            if (!result.Success)
                return ApiResult<InitiateDto>.Failure(result.ErrorMessage!);

            booking.Status = BookingStatus.Pending;
            _unitOfWork.BookingRepository.Update(booking);

            await _unitOfWork.PaymentRepository.AddAsync(new Payment
            {
                BookingId = booking.BookingId,
                Method = request.Method,
                TotalPrice = booking.TotalPrice,
                Status = PaymentStatus.Pending
            });

            await _unitOfWork.SaveChangesAsync();

            return ApiResult<InitiateDto>.Success(new InitiateDto { PaymentUrl = result.PaymentUrl });
        }
    }
}