using FluentValidation;
using Examination_System.ViewModels.Choice;
using static Examination_System.Models.Question;

namespace Examination_System.Validators.ViewModelValidators.Questions
{

    public class ChoiceViewModelValidator : AbstractValidator<ChoiceViewModel>
    {
        public ChoiceViewModelValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Choice text is required")
                .MaximumLength(200).WithMessage("Choice text cannot exceed 200 characters");
        }
    }
}