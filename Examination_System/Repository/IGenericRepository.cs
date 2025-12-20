using Examination_System.Models;
using Examination_System.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Repository
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        Task<T> GetById(int id);
        Task<List<T>> GetAll();
        Task Add(T entity);
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> GetAllWithSpecificationAsync(ISpecification<T> specifications);
        Task<T> GetByIdWithSpecification(ISpecification<T> spec);
    }
}
