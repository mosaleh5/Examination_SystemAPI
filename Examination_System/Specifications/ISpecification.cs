using Examination_System.Models;
using System.Linq.Expressions;

namespace Examination_System.Specifications
{
    public interface ISpecification<T,Tkey> where T :class , IBaseModel<Tkey>
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        Func<IQueryable<T>, IOrderedQueryable<T>> AdvancedOrderBy { get; }

        public List<Expression<Func<T, object>>> IncludeExpressions { get; }
        public List<string> IncludeStrings { get; set; } 
    }
}