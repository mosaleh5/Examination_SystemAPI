using FluentValidation;
using Examination_System.DTOs.Course;

namespace Examination_System.Validators.DTOValidators.Course
{
    public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
    {
        public UpdateCourseDtoValidator()
        {
            RuleFor(x => x.ID)
                .NotNull().WithMessage("Course ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .MaximumLength(200).WithMessage("Course name cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Course description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

        }
    }
}