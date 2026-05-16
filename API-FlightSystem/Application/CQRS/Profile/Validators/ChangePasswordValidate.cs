using Application.CQRS.Profile.Commands.ChangePassword;
using FluentValidation;

namespace Application.CQRS.Profile.Validators
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Mật khẩu hiện tại không được để trống");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Mật khẩu mới không được để trống")
                .MinimumLength(8).WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự")
                .Matches("[A-Z]").WithMessage("Mật khẩu mới phải có ít nhất 1 chữ hoa")
                .Matches("[0-9]").WithMessage("Mật khẩu mới phải có ít nhất 1 chữ số")
                .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu mới phải có ít nhất 1 ký tự đặc biệt")
                .NotEqual(x => x.CurrentPassword).WithMessage("Mật khẩu mới không được trùng mật khẩu hiện tại");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống")
                .Equal(x => x.NewPassword).WithMessage("Xác nhận mật khẩu không khớp");
        }
    }
}
