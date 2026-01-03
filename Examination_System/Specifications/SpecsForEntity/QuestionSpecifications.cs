using Examination_System.Models;
using System.Linq.Expressions;

namespace Examination_System.Specifications.SpecsForEntity
{
    public class QuestionSpecifications :BaseSpecification<Question,int>
    {
        

        public QuestionSpecifications(Expression<Func<Question, bool>>? criteriaExpression)
            : base(criteriaExpression)
        {
            AddInclude(q => q.Choices);
            
        }

       
    }
}
