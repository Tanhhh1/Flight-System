using Application.CQRS.Services.Commands.Update;
using FluentValidation;


namespace Application.CQRS.Services.Validators
{
    public class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
    {
        public UpdateServiceCommandValidator()
        {
            RuleFor(x => x.ServiceName)
                .NotEmpty().WithMessage("Tên dịch vụ không được để trống.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Giá dịch vụ phải lớn hơn hoặc bằng 0.");
        }
    }
}
