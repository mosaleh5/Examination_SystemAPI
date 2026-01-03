using Examination_System.Models;
using Examination_System.Specifications;
using System.Linq.Expressions;

namespace Examination_System.Repository
{
    // For entities with int IDs
    public interface IGenericRepository<T , Tkey> where T : IBaseModel<Tkey>
    {
        Task<T> GetByIdAsync(Tkey Tkey);
        IQueryable<T> GetAll();
        Task AddAsync(T entity);
        Task<T> UpdatePartialAsync(T entity);

        Task<T> GetById(Tkey id);
        
        // ExecuteUpdate method for efficient single-field updates
        Task<int> ExecuteUpdateAsync<TProperty>(
            Tkey id, 
            Func<T, TProperty> propertySelector, 
            TProperty newValue);
    }
    
  
}
