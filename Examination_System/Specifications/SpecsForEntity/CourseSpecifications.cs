using Examination_System.Models;

namespace Examination_System.Specifications.SpecsForEntity
{
    public class CourseSpecifications : BaseSpecification<Course>
    {
        private int id;

        public CourseSpecifications() : base(null)
        {
            AddInclude(c => c.CourseEnrollments);
            AddInclude(c => c.Exams);
            AddInclude(c => c.Instructor);
            ApplyAdvancedOrderBy(q => q.OrderBy(c => c.Name).ThenByDescending(c => c.CreatedAt));
        }

        public CourseSpecifications(int id) : base(c => c.ID == id)
        {
            {
                AddInclude(c => c.CourseEnrollments);
                AddInclude(c => c.Exams);
                AddInclude(c => c.Instructor);
            }
        }
    }
}
