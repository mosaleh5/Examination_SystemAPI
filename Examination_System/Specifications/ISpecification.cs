using Examination_System.Models;
using System.Linq.Expressions;

namespace Examination_System.Specifications
{
    public interface ISpecification<T> where T : BaseModel
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        Func<IQueryable<T>, IOrderedQueryable<T>> AdvancedOrderBy { get; }
        //public Expression<Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; set; }
        //public Expression<Func<T, object>> OrderByDescending { get; set; }
        public List<Expression<Func<T, object>>> IncludeExpressions { get; }

    }
}