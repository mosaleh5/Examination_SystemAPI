using FluentValidation;
using Examination_System.DTOs.Exam;

namespace Examination_System.Validators.DTOsValidators.Exam
{
    public class ReplaceExamQuestionsDtoValidator : AbstractValidator<ReplaceExamQuestionsDto>
    {
        public ReplaceExamQuestionsDtoValidator()
        {
            RuleFor(x => x.ExamId)
                .NotNull()
                .WithMessage("Exam ID must be greater than 0");

            RuleFor(x => x.QuestionIds)
                .NotNull()
                .WithMessage("Question IDs cannot be null")
                .NotEmpty()
                .WithMessage("At least one question ID is required")                
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Question IDs must be unique");

            RuleFor(x => x.InstructorId)
                .NotEmpty()
                .WithMessage("Instructor ID is required");
        }
    }
}