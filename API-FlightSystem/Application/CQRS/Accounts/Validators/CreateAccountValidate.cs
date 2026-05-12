using Application.CQRS.Accounts.Commands.Create;
using FluentValidation;

namespace Application.CQRS.Accounts.Validators
{
    public class CreateAccountValidate : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountValidate()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username không được để trống.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username chỉ được chứa chữ cái, số và dấu gạch dưới.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không đúng định dạng.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(8).WithMessage("Mật khẩu tối thiểu 8 ký tự.");
                
            RuleFor(x => x.Fullname)
                .NotEmpty().WithMessage("Họ tên không được để trống.")
                .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên chỉ được chứa chữ cái và khoảng trắng.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^0[0-9]{9}$").WithMessage("Số điện thoại phải bắt đầu bằng 0, gồm 10 chữ số.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Giới tính không được để trống.");

            RuleFor(x => x.RoleNames)
                .NotEmpty().WithMessage("Phải chọn ít nhất một vai trò.");
        }
    }
}
