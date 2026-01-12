using Examination_System.DTOs.Question;
using Examination_System.Models;
using FluentValidation;

namespace Examination_System.Validators.DTOValidators.Questions
{
    public class UpdateQuestionDtoValidator : AbstractValidator<UpdateQuestionDto>
    {
        public UpdateQuestionDtoValidator()
        {
            RuleFor(x => x.Id)
               .NotNull().WithMessage("Invalid question ID");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Question title is required")
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

            RuleFor(x => x.Mark)
                .GreaterThan(0).WithMessage("Mark must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Mark cannot exceed 100");


            RuleFor(x => x.Level)
                .Must(level => level >= QuestionLevel.Simple && level <= QuestionLevel.Hard)
                .WithMessage("Invalid question difficulty. Must be Simple (1), Medium (2), or Hard (3)");

            RuleFor(x => x.CourseId)
                .NotNull().WithMessage("Course ID is required");

            RuleFor(x => x.Choices)
                .NotEmpty().WithMessage("At least two choices are required")
                .Must(choices => choices != null && choices.Count >= 2)
                .WithMessage("At least two choices are required")
                .Must(choices => choices != null && choices.Count(c => c.IsCorrect) == 1)
                .WithMessage("Exactly one choice must be marked as correct");
        }
    }
}
