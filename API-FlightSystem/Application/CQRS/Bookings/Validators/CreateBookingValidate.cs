using Application.CQRS.Bookings.Commands.Booking;
using Application.CQRS.Bookings.DTOs;
using FluentValidation;

namespace Application.CQRS.Bookings.Validators
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.ClassId)
                .GreaterThan(0).WithMessage("ClassId không hợp lệ");

            RuleFor(x => x.FlightIds)
                .NotEmpty().WithMessage("Danh sách chuyến bay không được để trống")
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Danh sách chuyến bay không được trùng lặp")
                .Must(ids => ids.Count <= 3)
                .WithMessage("Không được đặt quá 3 chuyến bay trong một booking");

            RuleForEach(x => x.FlightIds)
                .GreaterThan(0).WithMessage("FlightId không hợp lệ");

            RuleFor(x => x.Passengers)
                .NotEmpty().WithMessage("Danh sách hành khách không được để trống");

            RuleForEach(x => x.Passengers)
                .SetValidator(new PassengerDtoValidator());
        }
    }

    public class PassengerDtoValidator : AbstractValidator<PassengerDto>
    {
        public PassengerDtoValidator()
        {
            RuleFor(x => x.TypeId)
                .GreaterThan(0).WithMessage("Mã loại hành khách không hợp lệ");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên hành khách không được để trống")
                .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự")
                .Matches(@"^[\p{L}\s]+$").WithMessage("Họ tên không được chứa ký tự đặc biệt hoặc số");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Giới tính không được để trống");

            RuleFor(x => x.Birthday)
                .NotEmpty().WithMessage("Ngày sinh không được để trống")
                .LessThan(DateTime.UtcNow).WithMessage("Ngày sinh phải là ngày trong quá khứ")
                .GreaterThan(DateTime.UtcNow.AddYears(-120)).WithMessage("Ngày sinh không hợp lệ");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Quốc tịch không được để trống")
                .MaximumLength(100).WithMessage("Tên quốc gia không được vượt quá 100 ký tự");
        }
    }
}