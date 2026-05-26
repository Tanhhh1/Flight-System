using Application.CQRS.Auth.Commands.SignUp;
using FluentValidation;


namespace Application.CQRS.Auth.Validators
{
    public class SignUpValidate : AbstractValidator<SignUpCommand>
    {
        public SignUpValidate()
        {
            RuleFor(x => x.Fullname)
                .NotEmpty().WithMessage("Họ tên không được để trống.")
                .MaximumLength(100).WithMessage("Họ tên không quá 100 ký tự.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username không được để trống.")
                .MinimumLength(3).WithMessage("Username tối thiểu 3 ký tự.")
                .MaximumLength(50).WithMessage("Username không quá 50 ký tự.")
                .Matches(@"^[a-zA-Z0-9\-._@+]+$")
                    .WithMessage("Username chứa ký tự không hợp lệ.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không đúng định dạng.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(8).WithMessage("Mật khẩu tối thiểu 8 ký tự.")
                .Matches(@"[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ hoa.")
                .Matches(@"[a-z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ thường.")
                .Matches(@"[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 chữ số.")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Mật khẩu phải có ít nhất 1 ký tự đặc biệt.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống.");
        }
    }
}
