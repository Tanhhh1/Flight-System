using Application.CQRS.Planes.Commands.Create;
using FluentValidation;

namespace Application.CQRS.Planes.Validators
{
    public class CreatePlaneValidate : AbstractValidator<CreatePlaneCommand>
    {
        public CreatePlaneValidate() 
        {
            RuleFor(x => x.PlaneModel)
                .NotEmpty().WithMessage("Tên máy bay không được để trống");

            RuleFor(x => x.AirlineId)
                .NotEmpty().WithMessage("Hãng bay không được để trống")
                .GreaterThan(0).WithMessage("Hãng bay không hợp lệ");
        }
    }
}
