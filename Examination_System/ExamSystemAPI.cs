using Examination_System.DTOs.Exam;
using Examination_System.Models;

namespace Examination_System.Mappings
{
    public static class ExamMapper
    {
        public static ExamDto ToDto(this Exam exam)
        {
            if (exam == null)
                return null;

            return new ExamDto
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                Duration = exam.Duration,
                TotalMarks = exam.TotalMarks,
                PassMarks = exam.PassMarks,
                StartDate = exam.StartDate,
                EndDate = exam.EndDate,
                IsActive = exam.IsActive,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt
            };
        }

        public static IEnumerable<ExamDto> ToDto(this IEnumerable<Exam> exams)
        {
            return exams?.Select(e => e.ToDto()) ?? Enumerable.Empty<ExamDto>();
        }

        public static Exam ToEntity(this ExamDto examDto)
        {
            if (examDto == null)
                return null;

            return new Exam
            {
                Id = examDto.Id,
                Title = examDto.Title,
                Description = examDto.Description,
                Duration = examDto.Duration,
                TotalMarks = examDto.TotalMarks,
                PassMarks = examDto.PassMarks,
                StartDate = examDto.StartDate,
                EndDate = examDto.EndDate,
                IsActive = examDto.IsActive
            };
        }
    }
}