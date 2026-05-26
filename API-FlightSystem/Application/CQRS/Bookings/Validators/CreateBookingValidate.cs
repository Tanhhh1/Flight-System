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

            RuleFor(x => x.TripType)
                .InclusiveBetween(1, 2).WithMessage("TripType phải là 1 (một chiều) hoặc 2 (khứ hồi)");

            RuleFor(x => x.Details)
                .NotEmpty().WithMessage("Danh sách hành khách không được để trống")
                .Must(details => details.Select(d => d.FlightId).Distinct().Count() <= 2)
                .WithMessage("Không được đặt quá 2 chuyến bay trong một booking");

            RuleForEach(x => x.Details)
                .SetValidator(new BookingDetailDtoValidator());
        }
    }

    public class BookingDetailDtoValidator : AbstractValidator<BookingDetailDto>
    {
        public BookingDetailDtoValidator()
        {
            RuleFor(x => x.FlightId)
                .GreaterThan(0).WithMessage("FlightId không hợp lệ");

            RuleFor(x => x.Passenger)
                .NotNull().WithMessage("Thông tin hành khách không được để trống")
                .SetValidator(new PassengerDtoValidator());
        }
    }

    public class PassengerDtoValidator : AbstractValidator<PassengerDto>
    {
        public PassengerDtoValidator()
        {
            RuleFor(x => x.TypeId)
                .GreaterThan(0).WithMessage("TypeId hành khách không hợp lệ");

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