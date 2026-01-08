using FluentValidation;
using Examination_System.ViewModels.Question;
using Examination_System.ViewModels.Choice;

namespace Examination_System.Validators.ViewModelValidators.Questions
{
    public class CreateQuestionViewModelValidator : AbstractValidator<CreateQuestionViewModel>
    {
        public CreateQuestionViewModelValidator()
        {
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
                .Must(HaveAtLeastTwoChoices).WithMessage("At least two choices are required")
                .Must(HaveExactlyOneCorrectChoice).WithMessage("Exactly one choice must be marked as correct");
        }

        private bool HaveAtLeastTwoChoices(ICollection<ChoiceViewModel>? choices)
        {
            return choices != null && choices.Count >= 2;
        }

        private bool HaveExactlyOneCorrectChoice(ICollection<ChoiceViewModel>? choices)
        {
            if (choices == null) return false;
            return choices.Count(c => c.IsCorrect) == 1;
        }
    }
}