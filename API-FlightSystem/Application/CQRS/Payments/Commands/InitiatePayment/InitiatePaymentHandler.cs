using Application.Common;
using Application.CQRS.Payments.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Payments.Commands.InitiatePayment
{
    public class InitiatePaymentHandler : IRequestHandler<InitiatePaymentCommand, ApiResult<InitiateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _gateway;

        public InitiatePaymentHandler(IUnitOfWork unitOfWork, IPaymentGateway gateway)
        {
            _unitOfWork = unitOfWork;
            _gateway = gateway;
        }

        public async Task<ApiResult<InitiateDto>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(request.BookingId);

            if (booking is null)
                return ApiResult<InitiateDto>.Failure("Không tìm thấy mã đơn đặt vé");

            if (booking.Status != BookingStatus.Pending)
                return ApiResult<InitiateDto>.Failure("Mã đơn đặt vé không ở trạng thái chờ thanh toán");

            var result = await _gateway.CreatePaymentUrlAsync(new PaymentRequest(
                BookingId: booking.BookingId,
                BookingCode: booking.BookingCode,
                Amount: booking.TotalPrice,
                Description: $"Thanh toan booking {booking.BookingCode}",
                ReturnUrl: request.ReturnUrl,
                IpAddress: request.ClientIp,
                Method: request.Method
            ));

            if (!result.Success)
                return ApiResult<InitiateDto>.Failure(result.ErrorMessage!);

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