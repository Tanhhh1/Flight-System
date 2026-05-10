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
        }
    }
}
