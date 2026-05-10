using Application.CQRS.Planes.Commands.Update;
using FluentValidation;

namespace Application.CQRS.Planes.Validators
{
    public class UpdatePlaneValidate : AbstractValidator<UpdatePlaneCommand>
    {
        public UpdatePlaneValidate()
        {
            RuleFor(x => x.PlaneModel)
                .NotEmpty().WithMessage("Tên máy bay không được để trống");

            RuleFor(x => x.AirlineId)
                .NotEmpty().WithMessage("Hãng bay không được để trống")
                .GreaterThan(0).WithMessage("Hãng bay không hợp lệ");
        }
    }
}
