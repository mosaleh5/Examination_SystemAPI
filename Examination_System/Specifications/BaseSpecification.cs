using System.Linq.Expressions;
using Examination_System.Models;

namespace Examination_System.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseModelGuid
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> IncludeExpressions { get; set; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; set; } = new List<string>();
        public Func<IQueryable<T>, IOrderedQueryable<T>> AdvancedOrderBy { get; private set; }

        public BaseSpecification()
        {
        }

        public BaseSpecification(Expression<Func<T, bool>>? criteriaExpression): this() 
        {
            Criteria = criteriaExpression;
        }

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            IncludeExpressions.Add(includeExpression);
        }

        // Add this method for nested includes
        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected void ApplyAdvancedOrderBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderByFunc)
        {
            AdvancedOrderBy = orderByFunc;
        }
    }
}
