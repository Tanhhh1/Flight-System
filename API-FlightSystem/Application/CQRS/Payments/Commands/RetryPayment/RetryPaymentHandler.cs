using Application.Common;
using Application.CQRS.Payments.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Payments.Commands.RetryPayment
{
    internal class RetryPaymentHandler : IRequestHandler<RetryPaymentCommand, ApiResult<InitiateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<IPaymentGateway> _gateways;
        private const int MaxRetryCount = 3;

        public RetryPaymentHandler(IUnitOfWork unitOfWork, IEnumerable<IPaymentGateway> gateways)
        {
            _unitOfWork = unitOfWork;
            _gateways = gateways;
        }

        public async Task<ApiResult<InitiateDto>> Handle(
            RetryPaymentCommand request,
            CancellationToken cancellationToken)
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

            var gateway = _gateways.FirstOrDefault(g => g.Method == request.Method);

            if (gateway is null)
                return ApiResult<InitiateDto>.Failure("Phương thức thanh toán không được hỗ trợ");

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

            var result = await gateway.CreatePaymentUrlAsync(new PaymentRequest(
                BookingId: booking.BookingId,
                BookingCode: booking.BookingCode,
                Amount: booking.TotalPrice,
                Description: $"Thanh toan lai booking {booking.BookingCode}",
                ReturnUrl: request.ReturnUrl,
                IpAddress: request.ClientIp
            ));

            if (!result.Success)
                return ApiResult<InitiateDto>.Failure(result.ErrorMessage!);

            return ApiResult<InitiateDto>.Success(new InitiateDto { PaymentUrl = result.PaymentUrl });
        }
    }
}
