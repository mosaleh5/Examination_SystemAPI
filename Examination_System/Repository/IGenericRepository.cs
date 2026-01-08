using Examination_System.Models;
using Examination_System.Specifications;
using System.Linq.Expressions;

namespace Examination_System.Repository
{
    /// <summary>
    /// Generic repository interface for data access operations.
    /// All methods automatically filter soft-deleted records (IsDeleted = true).
    /// </summary>
    public interface IGenericRepository<T, Tkey> where T : class, IBaseModel<Tkey>
    {
        Task<T> GetByIdAsync(Tkey id);

        IQueryable<T> GetAll();

        Task AddAsync(T entity);


        Task<T> UpdatePartialAsync(T entity);

      
        Task<T> GetById(Tkey id);

       
        IQueryable<T> GetByCriteria(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAllWithSpecificationAsync(ISpecification<T, Tkey> specifications);

        Task<T> GetByIdWithSpecification(ISpecification<T, Tkey> specification);

        Task<bool> IsExistsAsync(Tkey id);
        Task<bool> IsExistsbyCriteriaAsync(Expression<Func<T, bool>> predicate);

        Task<bool> DeleteAsync(Tkey id);

        Task<int> ExecuteUpdateAsync<TProperty>(
            Tkey id,
            Func<T, TProperty> propertySelector,
            TProperty newValue);
    }
}
