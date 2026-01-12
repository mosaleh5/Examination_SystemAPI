using System.Linq.Expressions;
using Examination_System.Models;
using Examination_System.Specifications;
using Microsoft.EntityFrameworkCore.Query;

namespace Examination_System.Repository
{
    /// <summary>
    /// Generic repository interface for data access operations.
    /// All methods automatically filter soft-deleted records (IsDeleted = true).
    /// </summary>
    public interface IGenericRepository<T> where T : BaseModelGuid
    {
        Task<T> GetByIdAsync(Guid id);

        IQueryable<T> GetAll();

        Task AddAsync(T entity);


        Task<T> UpdatePartialAsync(T entity);

      
        Task<T> GetById(Guid id);

       
        IQueryable<T> GetByCriteria(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAllWithSpecificationAsync(ISpecification<T> specifications);

        IQueryable<T> GetByIdWithSpecification(ISpecification<T> specification);

        Task<bool> IsExistsAsync(Guid id);
        Task<bool> IsExistsByCriteriaAsync(Expression<Func<T, bool>> predicate);

        Task<bool> DeleteAsync(Guid id);

        Task<int> ExecuteUpdateAsync<TProperty>(
            Guid id,
            Func<T, TProperty> propertySelector,
            TProperty newValue);

        Task<int> ExecuteUpdateAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls)
            where TEntity : class;
    }
}
