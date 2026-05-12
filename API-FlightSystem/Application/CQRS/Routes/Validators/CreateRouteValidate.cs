using Application.CQRS.Routes.Commands.Create;
using FluentValidation;

namespace Application.CQRS.Routes.Validators
{
    public class CreateRouteCommandValidator : AbstractValidator<CreateRouteCommand>
    {
        public CreateRouteCommandValidator()
        {
            RuleFor(x => x.OriginAirportId)
                .GreaterThan(0).WithMessage("Sân bay đi không hợp lệ");

            RuleFor(x => x.DestinationAirportId)
                .GreaterThan(0).WithMessage("Sân bay đến không hợp lệ");

            RuleFor(x => x)
                .Must(x => x.OriginAirportId != x.DestinationAirportId)
                .WithMessage("Sân bay đi và sân bay đến không được trùng nhau");

            RuleFor(x => x.FlightDuration)
                .GreaterThan(0).WithMessage("Thời gian bay phải lớn hơn 0")
                .LessThanOrEqualTo(1440).WithMessage("Thời gian bay không được quá 1440 phút (24 giờ)");
        }
    }
}
