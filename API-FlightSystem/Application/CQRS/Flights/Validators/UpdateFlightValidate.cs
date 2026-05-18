using Application.CQRS.Flights.Commands.Update;
using FluentValidation;

namespace Application.CQRS.Flights.Validators
{
    public class UpdateFlightValidator : AbstractValidator<UpdateFlightCommand>
    {
        public UpdateFlightValidator()
        {
            RuleFor(x => x.FlightId)
                .GreaterThan(0).WithMessage("Mã chuyến bay không hợp lệ.");

            RuleFor(x => x.PlaneId)
                .GreaterThan(0).WithMessage("Vui lòng chọn máy bay.");

            RuleFor(x => x.RouteId)
                .GreaterThan(0).WithMessage("Vui lòng chọn tuyến bay.");

            RuleFor(x => x.DepartureTime)
                .GreaterThan(DateTime.UtcNow).WithMessage("Giờ khởi hành không hợp lệ.");

            RuleFor(x => x.SeatPrices)
                .NotEmpty().WithMessage("Phải nhập thông tin giá vé.")
                .Must(list => list.Select(s => s.ClassId).Distinct().Count() == list.Count)
                .WithMessage("Hạng ghế trong giá vé không được trùng lặp.");

            RuleForEach(x => x.SeatPrices).ChildRules(seat =>
            {
                seat.RuleFor(s => s.ClassId).GreaterThan(0).WithMessage("Hạng ghế không hợp lệ.");
                seat.RuleFor(s => s.Price).GreaterThan(0).WithMessage("Giá vé phải lớn hơn 0.");
            });

            RuleFor(x => x.Services)
                .Must(list => list.Select(s => s.ServiceId).Distinct().Count() == list.Count)
                .WithMessage("Danh sách dịch vụ không được trùng lặp.")
                .When(x => x.Services.Count > 0);

            RuleForEach(x => x.Services).ChildRules(service =>
            {
                service.RuleFor(s => s.ServiceId).GreaterThan(0).WithMessage("Dịch vụ không hợp lệ.");
            });

            RuleForEach(x => x.Segments).ChildRules(seg =>
            {
                seg.RuleFor(s => s.RouteId).GreaterThan(0).WithMessage("Tuyến bay của chặng không hợp lệ.");
                seg.RuleFor(s => s.DepartureTime).GreaterThan(DateTime.UtcNow).WithMessage("Giờ khởi hành chặng không hợp lệ.");
            }).When(x => x.Segments.Count > 0);

            RuleFor(x => x.Segments)
                .Must((command, segments) => segments.All(s => s.RouteId != command.RouteId))
                .WithMessage("Tuyến bay của chặng không được trùng với tuyến bay chính.")
                .When(x => x.Segments.Count > 0);

            RuleFor(x => x.Segments)
                .Must(segments => segments.Select(s => s.RouteId).Distinct().Count() == segments.Count)
                .WithMessage("Các chặng không được dùng trùng tuyến bay.")
                .When(x => x.Segments.Count > 1);
        }
    }
}