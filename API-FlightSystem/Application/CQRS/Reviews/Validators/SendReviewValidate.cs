using Application.CQRS.Reviews.Commands.Send;
using FluentValidation;

namespace Application.CQRS.Reviews.Validators
{
    public class SendReviewValidate : AbstractValidator<SendReviewCommand>
    {
        public SendReviewValidate()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung đánh giá không được để trống.")
                .MaximumLength(1000).WithMessage("Nội dung đánh giá không được vượt quá 1000 ký tự.");
        }
    }
}
