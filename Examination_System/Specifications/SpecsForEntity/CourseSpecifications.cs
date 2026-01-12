using Examination_System.Models;
using System.Linq.Expressions;

namespace Examination_System.Specifications.SpecsForEntity
{
    public class CourseSpecifications : BaseSpecification<Course>
    {
        /// <summary>
        /// private int id;
        /// </summary>

        public CourseSpecifications(Expression<Func<Course, bool>> Criteria) : base(Criteria)
        {
         
            AddInclude(c => c.CourseEnrollments);
            AddInclude(c => c.Exams);
            AddInclude(c => c.Instructor);
            ApplyAdvancedOrderBy(q => q.OrderBy(c => c.Name).ThenByDescending(c => c.CreatedAt));
        }

    }
}
