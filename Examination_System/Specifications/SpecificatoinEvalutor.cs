using Examination_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Specifications
{
    public class SpecificatoinEvalutor<T> where T : BaseModelGuid
    {
        public static IQueryable<T> CreatQuery(IQueryable<T> Query, ISpecification<T> specs)
        {
            if (specs.Criteria != null)
            {
                Query = Query.Where(specs.Criteria);
            }

            if (specs.AdvancedOrderBy != null)
            {
                Query = specs.AdvancedOrderBy(Query);
            }

            if (specs.IncludeExpressions is not null && specs.IncludeExpressions.Count > 0)
            {
                Query = specs.IncludeExpressions
                    .Aggregate(Query, (current, include) => current.Include(include));
            }

           
            if (specs.IncludeStrings is not null && specs.IncludeStrings.Count > 0)
            {
                Query = specs.IncludeStrings
                    .Aggregate(Query, (current, include) => current.Include(include));
            }

            return Query.Where(s=>s.IsDeleted == false);
        }
    }
}
