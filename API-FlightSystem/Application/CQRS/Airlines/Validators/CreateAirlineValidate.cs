using Application.CQRS.Airlines.Commands.Create;
using FluentValidation;

namespace Application.CQRS.Airlines.Validators
{
    public class CreateAirlineValidate : AbstractValidator<CreateAirlineCommand>
    {
        public CreateAirlineValidate()
        {
            RuleFor(x => x.AirlineName)
                .NotEmpty().WithMessage("Tên hãng hàng không không được để trống");

            RuleFor(x => x.AirlineCode)
                .NotEmpty().WithMessage("Mã hãng hàng không không được để trống")
                .Length(2, 3).WithMessage("Mã hãng hàng không phải từ 2 đến 3 ký tự")
                .Matches("^[A-Z0-9]*$").WithMessage("Mã hãng hàng không chỉ được chứa ký tự chữ in hoa và số");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Quốc gia không được để trống");
        }
    }
}
