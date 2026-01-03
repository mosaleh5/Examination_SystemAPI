using Examination_System.Models;
using System.Linq.Expressions;

namespace Examination_System.Specifications.SpecsForEntity
{
    public class ExamAttemptSpecifications : BaseSpecification<ExamAttempt , int>
    {
        public ExamAttemptSpecifications() 
        {
            ApplyIncludes();
        }

        public ExamAttemptSpecifications(Expression<Func<ExamAttempt, bool>> Criteria) : base(Criteria)
        {
            ApplyIncludes();
        }

        private void ApplyIncludes()
        {
            AddInclude(c => c.Exam);
            AddInclude(c => c.Student);

            // Include StudentAnswers with nested Question, Choices, and SelectedChoice
            AddInclude("StudentAnswers.Question.Choices");
            AddInclude("StudentAnswers.SelectedChoice");
        }
    }
}
