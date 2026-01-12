using FluentValidation;
using Examination_System.DTOs.Exam;

namespace Examination_System.Validators.DTOsValidators.Exam
{
    public class RemoveQuestionFromExamDtoValidator : AbstractValidator<RemoveQuestionFromExamDto>
    {
        public RemoveQuestionFromExamDtoValidator()
        {
            RuleFor(x => x.ExamId)
                .NotNull()
                .WithMessage("Exam ID must be greater than 0");

            RuleFor(x => x.QuestionId)
                .NotNull()
                .WithMessage("Question ID must be greater than 0");

            RuleFor(x => x.InstructorId)
                .NotEmpty()
                .WithMessage("Instructor ID is required");
        }
    }
}