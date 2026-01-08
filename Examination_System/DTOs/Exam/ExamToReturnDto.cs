using Examination_System.DTOs.Question;
using Examination_System.Models;
using System.ComponentModel.DataAnnotations;


namespace Examination_System.DTOs.Exam
{
    public class ExamToReturnDto
    {

         public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int DurationInMinutes { get; set; }
        public int Fullmark { get; set; }
        public int PassingPercentage { get; set; }
        public string ExamType { get; set; } // "Quiz" or "Final"
        public int QuestionsCount { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string InstructorId { get; set; }
        public string InstructorName { get; set; }

        public bool isActive { get; set; }

        public DateTime CreatedAt { get; internal set; }

        public bool IsAutomatic { get; set; }
        public ICollection<QuestionToReturnDto>? ExamQuestions { get; set; }

    }
}
