using FluentValidation;
using Examination_System.DTOs.Course;

namespace Examination_System.Validators.DTOValidators.Course
{
    public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .MaximumLength(200).WithMessage("Course name cannot exceed 200 characters");

            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("Instructor ID is required");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Course description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

  
        }
    }
}