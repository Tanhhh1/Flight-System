using Application.Common;
using Application.CQRS.Payments.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Payments.Commands.InitiatePayment
{
    public class InitiatePaymentCommand : IRequest<ApiResult<InitiateDto>>
    {
        public int BookingId { get; set; }
        public PaymentMethod Method { get; set; }
        public string ReturnUrl { get; set; } = string.Empty;
        public string ClientIp { get; set; } = string.Empty;
    }
}