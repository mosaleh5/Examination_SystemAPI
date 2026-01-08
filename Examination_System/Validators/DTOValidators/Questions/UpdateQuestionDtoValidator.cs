using Examination_System.DTOs.Question;
using FluentValidation;

namespace Examination_System.Validators.DTOValidators.Questions
{
    public class UpdateQuestionDtoValidator : AbstractValidator<UpdateQuestionDto>
    {
        public UpdateQuestionDtoValidator()
        {
            RuleFor(x => x.Id)
               .GreaterThan(0).WithMessage("Invalid question ID");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Question title is required")
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

            RuleFor(x => x.Mark)
                .GreaterThan(0).WithMessage("Mark must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Mark cannot exceed 100");

            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Invalid question level");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("Course ID is required");

            RuleFor(x => x.Choices)
                .NotEmpty().WithMessage("At least two choices are required")
                .Must(choices => choices != null && choices.Count >= 2)
                .WithMessage("At least two choices are required")
                .Must(choices => choices != null && choices.Count(c => c.IsCorrect) == 1)
                .WithMessage("Exactly one choice must be marked as correct");
        }
    }
}
