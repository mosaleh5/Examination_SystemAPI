using FluentValidation;

namespace Examination_System.Validators.ViewModelValidators.Course
{
    public class EnrollStudentRequest
    {
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
    }

    public class EnrollStudentViewModelValidator : AbstractValidator<EnrollStudentRequest>
    {
        public EnrollStudentViewModelValidator()
        {
            RuleFor(x => x.CourseId)
                .NotNull().WithMessage("Invalid course ID");

            RuleFor(x => x.StudentId)
                .NotNull().WithMessage("Student ID cannot be null or empty");
               
        }
    }
}