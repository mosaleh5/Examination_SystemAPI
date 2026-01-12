using Examination_System.DTOs.Exam;
using FluentValidation;
using System.Data;

namespace Examination_System.Validators.DTOsValidators.Exam
{
    public class GetExamByIdValidator : AbstractValidator<GetExamByIdDto>
    {
        public GetExamByIdValidator()
        { 
        
            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("InstructorId is required.")
                .NotEqual(Guid.Empty).WithMessage("InstructorId cannot be an empty GUID.");
            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("ExamId is required.")
                .NotEqual(Guid.Empty).WithMessage("ExamId cannot be an empty GUID.");


        }
    }
}
