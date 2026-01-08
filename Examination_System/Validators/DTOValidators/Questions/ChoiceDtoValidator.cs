using Examination_System.DTOs.Question;
using FluentValidation;

namespace Examination_System.Validators.DTOValidators.Questions
{
    public class ChoiceDtoValidator : AbstractValidator<ChoiceDto>
    {
        public ChoiceDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Choice text is required")
                .MaximumLength(200).WithMessage("Choice text cannot exceed 200 characters");
        }
    }
}
