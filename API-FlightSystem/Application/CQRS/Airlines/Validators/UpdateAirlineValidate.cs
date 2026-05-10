using Application.CQRS.Airlines.Commands.Update;
using FluentValidation;

namespace Application.CQRS.Airlines.Validators
{
    public class UpdateAirlineValidate : AbstractValidator<UpdateAirlineCommand>
    {
        public UpdateAirlineValidate()
        {
            RuleFor(x => x.AirlineName)
                .NotEmpty().WithMessage("Tên hãng hàng không không được để trống");
        }
    }
}
