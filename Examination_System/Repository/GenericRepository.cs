using Examination_System.Data;
using Examination_System.Models;
using Examination_System.Specifications;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Repository
{
    public class GenericRepository<T> :IGenericRepository<T> where T : BaseModel
    {
        Context _context;
        DbSet<T> _dbSet;
        public GenericRepository(Context context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async  Task<T> GetById(int id)
        {

            var res = await _dbSet.Where(c => c.ID == id).FirstOrDefaultAsync();
            return res;
        }
        public async Task<List<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<T> UpdateAsync(T TEntity)
        {
            _dbSet.Attach(TEntity);
            _context.SaveChanges();
            return TEntity;
        }
        public async Task <IEnumerable<T>> GetAllWithSpecificationAsync(ISpecification<T> specifications) 
        {
            return await  ApplySpecification(specifications).ToListAsync();
        }
        private IQueryable<T> ApplySpecification(ISpecification<T> Spec)
        {
            return  SpecificatoinEvalutor<T>.CreatQuery(_dbSet, Spec);
        }

        public async Task<T> GetByIdWithSpecification(ISpecification<T> Spec)
        {
            return await ApplySpecification(Spec).FirstOrDefaultAsync();
        }
    }
}
