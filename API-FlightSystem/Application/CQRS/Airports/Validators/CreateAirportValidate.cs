using Application.CQRS.Airports.Commands.Create;
using FluentValidation;

namespace Application.CQRS.Airports.Validators
{
    public class CreateAirportValidate : AbstractValidator<CreateAirportCommand>
    {
        public CreateAirportValidate()
        {
            RuleFor(x => x.AirportCode)
                .NotEmpty().WithMessage("Airport code không được để trống")
                .MaximumLength(10).WithMessage("Airport code không được quá 10 ký tự")
                .Matches("^[A-Z]{3}$").WithMessage("Airport code phải là 3 chữ cái in hoa (VD: HAN, SGN)");

            RuleFor(x => x.AirportName)
                .NotEmpty().WithMessage("Tên airport không được để trống")
                .MaximumLength(200).WithMessage("Tên airport không được quá 200 ký tự");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Thành phố không được để trống")
                .MaximumLength(100).WithMessage("Thành phố không được quá 100 ký tự");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Quốc gia không được để trống")
                .MaximumLength(3).WithMessage("Mã quốc gia không được quá 3 ký tự");
        }
    }
}
