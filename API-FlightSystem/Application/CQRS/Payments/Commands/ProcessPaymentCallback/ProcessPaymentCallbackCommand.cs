using Application.Common;
using Application.CQRS.Payments.DTOs;
using MediatR;

namespace Application.CQRS.Payments.Commands.ProcessPaymentCallback
{
    public class ProcessPaymentCallbackCommand : IRequest<ApiResult<ProcessCallbackDto>>
    {
        public string Method { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
