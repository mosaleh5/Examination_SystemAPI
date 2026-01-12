using Examination_System.DTOs.Question;
using Examination_System.Models;
using FluentValidation;
using static Examination_System.Models.Question;

namespace Examination_System.Validators.DTOValidators.Questions
{
    public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
    {
        public CreateQuestionDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Question text is required")
                .MaximumLength(500);

            RuleFor(x => x.CourseId)
                .NotNull()
                .WithMessage("Valid course ID is required");

            RuleFor(x => x.Level)
                .Must(level => level >= QuestionLevel.Simple && level <= QuestionLevel.Hard)
                .WithMessage("Invalid question difficulty. Must be Simple (1), Medium (2), or Hard (3)");

            RuleFor(x => x.Choices)
                .NotEmpty()
                .Must(x => x.Count() >= 2)
                .WithMessage("Choices are required for multiple choice questions")
                .Must(x=> x.Count(x => x.IsCorrect) == 1)
                .WithMessage("Only one choice must be Correct");         
        }

    }
}