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
        private readonly IEnumerable<IPaymentGateway> _gateways;

        public InitiatePaymentHandler(IUnitOfWork unitOfWork, IEnumerable<IPaymentGateway> gateways)
        {
            _unitOfWork = unitOfWork;
            _gateways = gateways;
        }

        public async Task<ApiResult<InitiateDto>> Handle(
            InitiatePaymentCommand request,
            CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.BookingRepository
                .GetByIdAsync(request.BookingId);

            if (booking is null)
                return ApiResult<InitiateDto>.Failure("Không tìm thấy booking");

            if (booking.Status != BookingStatus.Pending)
                return ApiResult<InitiateDto>.Failure("Booking không ở trạng thái chờ thanh toán");

            var gateway = _gateways.FirstOrDefault(g => g.Method == request.Method);

            if (gateway is null)
                return ApiResult<InitiateDto>.Failure("Phương thức thanh toán không được hỗ trợ");

            var result = await gateway.CreatePaymentUrlAsync(new PaymentRequest(
                BookingId: booking.BookingId,
                BookingCode: booking.BookingCode,
                Amount: booking.TotalPrice,
                Description: $"Thanh toan booking {booking.BookingCode}",
                ReturnUrl: request.ReturnUrl,
                IpAddress: request.ClientIp
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
