using Application.CQRS.Flights.Commands.Update;
using FluentValidation;

namespace Application.CQRS.Flights.Validators
{
    public class UpdateFlightValidate : AbstractValidator<UpdateFlightCommand>
    {
        public UpdateFlightValidate()
        {
            RuleFor(x => x.FlightId)
                .GreaterThan(0).WithMessage("Mã chuyến bay không hợp lệ");

            RuleFor(x => x.PlaneId)
                .GreaterThan(0).WithMessage("Vui lòng chọn máy bay");

            RuleFor(x => x.RouteId)
                .GreaterThan(0).WithMessage("Vui lòng chọn tuyến bay");

            RuleFor(x => x.DepartureTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Thời gian khởi hành phải lớn hơn thời điểm hiện tại");

            RuleFor(x => x.SeatPrices)
                .NotEmpty().WithMessage("Vui lòng nhập giá vé");

            RuleForEach(x => x.SeatPrices).ChildRules(seat =>
            {
                seat.RuleFor(x => x.ClassId)
                    .GreaterThan(0).WithMessage("ClassId không hợp lệ");
                seat.RuleFor(x => x.Price)
                    .GreaterThan(0).WithMessage("Giá vé phải lớn hơn 0");
            });

            RuleForEach(x => x.Segments).ChildRules(segment =>
            {
                segment.RuleFor(x => x.RouteId)
                    .GreaterThan(0).WithMessage("Vui lòng chọn tuyến bay cho chặng");

                segment.RuleFor(x => x.DepartureTime)
                    .GreaterThan(DateTime.UtcNow).WithMessage("Thời gian khởi hành của chặng phải lớn hơn hiện tại");
            });
        }
    }
}
