using Examination_System.Models;
using Examination_System.ViewModels.Question;
using FluentValidation;

namespace Examination_System.Validators.ViewModelValidators.Questions
{
    public class UpdateQuestionViewModelValidator : AbstractValidator<UpdateQuestionViewModel>
    {
        public UpdateQuestionViewModelValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("Invalid question ID");

            RuleFor(x => x.Text)
                
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

            RuleFor(x => x.Mark)
                .GreaterThan(0).WithMessage("Mark must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Mark cannot exceed 100");
            RuleFor(x => x.Level)
                         .Must(level => level >= QuestionLevel.Simple && level <= QuestionLevel.Hard)
                         .WithMessage("Invalid question difficulty. Must be Simple (1), Medium (2), or Hard (3)");



            RuleFor(x => x.Choices)
              
                .Must(choices => choices != null && choices.Count >= 2)
                .WithMessage("At least two choices are required")
                .Must(choices => choices != null && choices.Count(c => c.IsCorrect) == 1)
                .WithMessage("Exactly one choice must be marked as correct");
        }
    }
}