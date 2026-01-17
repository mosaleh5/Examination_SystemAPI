using Examination_System.DTOs.Course;
using FluentValidation;

namespace Examination_System.Validators.DTOsValidators.Course
{
    public class CourseEnrollemtDtoValidator : AbstractValidator<CourseEnrollementDto>
    {
        public CourseEnrollemtDtoValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required.");
            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("InstructorId is required.");
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required.");
        }
    }
}
