using Application.CQRS.Profile.Commands.Update;
using FluentValidation;

namespace Application.CQRS.Profile.Validators
{
    public class UpdateProfileValidate : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileValidate()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không đúng định dạng.");

            RuleFor(x => x.Fullname)
                .NotEmpty().WithMessage("Họ tên không được để trống.")
                .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^0[0-9]{9}$").WithMessage("Số điện thoại phải bắt đầu bằng 0, gồm 10 chữ số.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }
}
