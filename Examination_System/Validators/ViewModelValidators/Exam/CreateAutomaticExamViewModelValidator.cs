using Examination_System.Models.Enums;
using Examination_System.ViewModels.Exam;
using FluentValidation;

namespace Examination_System.Validators.ViewModelValidators.Exam
{
    public class CreateAutomaticExamViewModelValidator : AbstractValidator<CreateAutomaticExamViewModel>
    {
        public CreateAutomaticExamViewModelValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Exam title is required")
                .MaximumLength(150).WithMessage("Title cannot exceed 150 characters");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Exam date is required")
                .Must(BeInFuture).WithMessage("Exam date must be in the future");

            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0).WithMessage("Duration must be at least 1 minute");

            RuleFor(x => x.Fullmark)
                .GreaterThan(0).WithMessage("Full mark must be greater than 0");


            RuleFor(x => x.ExamType)
                .Must(exam => exam == ExamType.Quiz || exam == ExamType.Final)
                .WithMessage("Exam type must be quiz or final");

            RuleFor(x => x.PassingPercentage)
                .InclusiveBetween(0, 100).WithMessage("Passing percentage must be between 0 and 100");

            RuleFor(x => x.QuestionsCount)
                .GreaterThan(0).WithMessage("Questions count must be at least 1");

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required");
        }

        private bool BeInFuture(DateTime date)
        {
            return date > DateTime.UtcNow;
        }
    }
}