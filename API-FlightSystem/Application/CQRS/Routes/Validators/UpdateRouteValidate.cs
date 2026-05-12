using Application.CQRS.Routes.Commands.Update;
using FluentValidation;

namespace Application.CQRS.Routes.Validators
{
    public class UpdateRouteValidate : AbstractValidator<UpdateRouteCommand>
    {
        public UpdateRouteValidate() 
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
                .LessThanOrEqualTo(1440).WithMessage("Thời gian bay không được vượt quá 1440 phút (24 giờ)");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Trạng thái tuyến bay không hợp lệ");
        }
    }
}
