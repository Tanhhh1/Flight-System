using Application.CQRS.Auth.Commands.SignIn;
using FluentValidation;

namespace Application.CQRS.Auth.Validators
{
    public class SignInValidate : AbstractValidator<SignInCommand>
    {
        public SignInValidate()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống");
        }
    }
}
