using Examination_System.Models;

namespace Examination_System.Specifications.SpecsForEntity
{
    public class ExamSpecifications : BaseSpecification<Exam>
    {
        public ExamSpecifications() : base(null)
        {
            AddInclude(e => e.Questions);
            AddInclude(e => e.Course);
            AddInclude(e => e.Instructor);
        }
    }
}
