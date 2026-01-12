using Examination_System.Models;
using System.Linq.Expressions;

namespace Examination_System.Specifications.SpecsForEntity
{
    public class ExamSpecifications : BaseSpecification<Exam>
    {
        public ExamSpecifications(Expression<Func<Exam, bool>> Criteria) : base(Criteria)
        {
            // Basic includes
            AddInclude(e => e.Course);
            AddInclude(e => e.Instructor);
            
            // Nested includes using string-based navigation
            AddInclude("ExamQuestions.Question.Choices");
            AddInclude("Instructor.User");
           
        }
    }
}
