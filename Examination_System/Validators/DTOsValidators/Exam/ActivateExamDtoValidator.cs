using FluentValidation;
using Examination_System.DTOs.Exam;

namespace Examination_System.Validators.DTOsValidators.Exam
{
    public class ActivateExamDtoValidator : AbstractValidator<ActivateExamDto>
    {
        public ActivateExamDtoValidator()
        {
            RuleFor(x => x.ExamId)
                .NotNull()
                .WithMessage("Exam ID must be greater than 0");

            RuleFor(x => x.InstructorId)
                .NotNull()
                .WithMessage("Instructor ID is required");
        }
    }
}