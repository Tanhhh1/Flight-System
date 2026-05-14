using Application.CQRS.Flights.Commands.Create;
using FluentValidation;

namespace Application.CQRS.Flights.Validators
{
    public class CreateFlightValidator : AbstractValidator<CreateFlightCommand>
    {
        public CreateFlightValidator()
        {
            RuleFor(x => x.PlaneId)
                .GreaterThan(0).WithMessage("Vui lòng chọn máy bay");

            RuleFor(x => x.RouteId)
                .GreaterThan(0).WithMessage("Vui lòng chọn tuyến bay");

            RuleFor(x => x.DepartureTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Giờ khởi hành không hợp lệ");

            RuleFor(x => x.ArrivalTime)
                .GreaterThan(x => x.DepartureTime).WithMessage("Giờ đến phải sau giờ khởi hành");

            RuleFor(x => x.SeatPrices)
                .NotEmpty().WithMessage("Phải nhập thông tin giá vé")
                .Must(list => list.Select(s => s.ClassId).Distinct().Count() == list.Count)
                .WithMessage("Hạng ghế trong giá vé không được trùng lặp");

            RuleForEach(x => x.SeatPrices).ChildRules(seat =>
            {
                seat.RuleFor(s => s.ClassId).InclusiveBetween(1, 4).WithMessage("Hạng ghế không hợp lệ");
                seat.RuleFor(s => s.Price).GreaterThan(0).WithMessage("Giá vé phải lớn hơn 0");
            });

            RuleFor(x => x.Services)
                .Must(list => list.Select(s => s.ServiceId).Distinct().Count() == list.Count)
                .WithMessage("Danh sách dịch vụ không được trùng lặp")
                .When(x => x.Services.Count > 0);

            RuleForEach(x => x.Services).ChildRules(service =>
            {
                service.RuleFor(s => s.ServiceId)
                    .GreaterThan(0).WithMessage("Dịch vụ không hợp lệ");
            });


            RuleForEach(x => x.Segments).ChildRules(seg =>
            {
                seg.RuleFor(s => s.RouteId)
                    .GreaterThan(0).WithMessage("Tuyến bay không hợp lệ");

                seg.RuleFor(s => s.DepartureTime)
                    .GreaterThan(DateTime.UtcNow).WithMessage("Giờ khởi hành chặng không hợp lệ");

                seg.RuleFor(s => s.ArrivalTime)
                    .GreaterThan(s => s.DepartureTime).WithMessage("Giờ hạ cánh chặng phải sau giờ khởi hành chặng");
            }).When(x => x.Segments.Count > 0);

            RuleForEach(x => x.Segments)
               .Must((command, seg) => seg.DepartureTime >= command.DepartureTime && seg.ArrivalTime <= command.ArrivalTime)
               .WithMessage("Thời gian chặng phải nằm trong khoảng thời gian chuyến bay")
               .When(x => x.Segments.Count > 0);


            RuleFor(x => x.Segments)
                .Must(segments =>
                {
                    for (int i = 0; i < segments.Count - 1; i++)
                    {
                        if (segments[i + 1].DepartureTime < segments[i].ArrivalTime)
                            return false;
                    }
                    return true;
                })
                .WithMessage("Thời gian các chặng bị chồng lấp nhau")
                .When(x => x.Segments.Count > 1);

            RuleFor(x => x.Segments)
                .Must((command, segments) =>
                    segments.All(s => s.RouteId != command.RouteId))
                .WithMessage("Tuyến bay của chặng không được trùng với tuyến bay chính")
                .When(x => x.Segments.Count > 0);
        }
    }
}
