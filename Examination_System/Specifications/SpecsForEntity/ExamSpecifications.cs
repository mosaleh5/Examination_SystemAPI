using Examination_System.Models;
using System.Linq.Expressions;

namespace Examination_System.Specifications.SpecsForEntity
{
    public class ExamSpecifications : BaseSpecification<Exam>
    {
        public ExamSpecifications(Expression<Func<Exam, bool>> Criteria) : base(Criteria)
        {
            // Basic includes - expression-based
            AddInclude(e => e.Course);
            AddInclude(e => e.Instructor);
            AddInclude(e => e.ExamQuestions);  
            
            // Nested includes using string-based navigation
            AddInclude("ExamQuestions.Question");
            AddInclude("ExamQuestions.Question.Choices");
            AddInclude("Instructor.User");
        }
    }
}
