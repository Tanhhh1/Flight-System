using Application.CQRS.Airports.Commands.Create;
using FluentValidation;

namespace Application.CQRS.Airports.Validators
{
    public class CreateAirportValidate : AbstractValidator<CreateAirportCommand>
    {
        public CreateAirportValidate()
        {
            RuleFor(x => x.AirportCode)
                .NotEmpty().WithMessage("Mã định danh sân bay không được để trống")
                .MaximumLength(10).WithMessage("Mã định danh sân bay không được quá 10 ký tự")
                .Matches("^[A-Z]{3}$").WithMessage("Mã định danh sân bay phải là 3 chữ cái in hoa (VD: HAN, SGN)");

            RuleFor(x => x.AirportName)
                .NotEmpty().WithMessage("Tên sân bay không được để trống");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Thành phố không được để trống");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Quốc gia không được để trống");
        }
    }
}
