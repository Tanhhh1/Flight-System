using Application.CQRS.Support.Commands.Create;
using Domain.Enums;
using FluentValidation;

namespace Application.CQRS.Support.Validators
{
    public class CreateSupportRequestValidator : AbstractValidator<CreateSupportRequestCommand>
    {
        public CreateSupportRequestValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0).WithMessage("BookingId không hợp lệ.");

            RuleFor(x => x.RequestType)
                .IsInEnum().WithMessage("Loại yêu cầu không hợp lệ.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Lý do không được để trống.")
                .MaximumLength(500).WithMessage("Lý do không được vượt quá 500 ký tự.");

            RuleFor(x => x.NewFlightId)
                .NotNull().WithMessage("Vui lòng chọn chuyến bay mới.")
                .GreaterThan(0).WithMessage("Chuyến bay mới không hợp lệ.")
                .When(x => x.RequestType == RequestType.Reschedule);
        }
    }
}
