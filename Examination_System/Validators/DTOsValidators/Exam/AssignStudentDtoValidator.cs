using Examination_System.DTOs.Exam;
using FluentValidation;

namespace Examination_System.Validators.DTOsValidators.Exam
{
    public class AssignStudentToExamDtoValidator : AbstractValidator<AssignStudentToExamDto>
    {
        public AssignStudentToExamDtoValidator() 
        {
        
            RuleFor(x => x.ExamId)
                .NotNull().WithMessage("ExamId must be greater than 0.");

            RuleFor(x => x.StudentId)
                .NotNull().WithMessage("StudentId is required.");

            RuleFor(x => x.InstructorId)
                .NotNull().WithMessage("InstructorId is required.");


        }
    }
}
