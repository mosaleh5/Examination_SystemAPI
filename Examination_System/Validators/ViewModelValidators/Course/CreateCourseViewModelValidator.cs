using FluentValidation;
using Examination_System.ViewModels.Course;

namespace Examination_System.Validators.ViewModelValidators.Course
{
    public class CreateCourseViewModelValidator : AbstractValidator<CreateCourseViewModel>
    {
        public CreateCourseViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .MaximumLength(200).WithMessage("Course name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Course description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

        }
    }
}