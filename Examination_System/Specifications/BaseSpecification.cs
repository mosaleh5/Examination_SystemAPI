using System.Linq.Expressions;
using Examination_System.Models;

namespace Examination_System.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseModel
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        //public Expression<Func<T, object>> OrderBy { get; set; } 
        // Ireplac it with this
        //public Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; set; }

        //public Expression<Func<T, object>> OrderByDescending { get; set; }
        public List<Expression<Func<T, object>>> IncludeExpressions { get; set; } = new List<Expression<Func<T, object>>>();
        public Func<IQueryable<T>, IOrderedQueryable<T>> AdvancedOrderBy { get; private set; }

        public BaseSpecification(Expression<Func<T, bool>> ?criteriaExpression)
        {
            Criteria = criteriaExpression;
        }

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            IncludeExpressions.Add(includeExpression);
        }


      
        protected void ApplyAdvancedOrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderByFunc)
        {
            AdvancedOrderBy = orderByFunc;
        }
    }
}
