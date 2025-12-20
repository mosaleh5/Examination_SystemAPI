using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Examination_System.Specifications
{
    public class SpecificatoinEvalutor<T> where T : BaseModel
    {
        public static IQueryable<T> CreatQuery(IQueryable<T> Query, ISpecification<T> specs)
        {
            if (specs.Criteria != null)
            {
                Query = Query.Where(specs.Criteria);
            }
            //if (specs.OrderBy != null)
            //{
            //    Query = Query.OrderBy(specs.OrderBy);
            //}
            //if (specs.OrderByDescending != null)
            //{
            //    Query = Query.OrderByDescending(specs.OrderByDescending);
            //}

            if (specs.AdvancedOrderBy != null)
            {
                Query = specs.AdvancedOrderBy(Query);
            }
            if (specs.IncludeExpressions is not null && specs.IncludeExpressions.Count > 0 )
            {
                Query = specs.IncludeExpressions
                    .Aggregate(Query, (current, include) => current.Include(include));
            }
            return Query;
        }
    }
}
