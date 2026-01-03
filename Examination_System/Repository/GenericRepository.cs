using Examination_System.Data;
using Examination_System.Models;
using Examination_System.Specifications;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Examination_System.Repository
{
    // For entities with int IDs
    public class GenericRepository<T, Tkey> : IGenericRepository<T, Tkey> where T : class, IBaseModel<Tkey>
    {
        private readonly Context _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(Context context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(Tkey id)
        {
            var res = await _dbSet.Where(c => c.Id.Equals(id) && !c.IsDeleted).FirstOrDefaultAsync();
            return res;
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.Where(e => !e.IsDeleted).AsQueryable<T>();
        }

        public async Task AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);

        }




        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);

        }
        public async Task<T> UpdatePartialAsync(T TEntity)
        {
            _dbSet.Attach(TEntity);
            _context.Entry(TEntity).State = EntityState.Modified;

            return TEntity;
        }
        public IQueryable<T> GetAllWithSpecificationAsync(ISpecification<T, Tkey> specifications)
        {
            return ApplySpecification(specifications).AsQueryable<T>();
        }
        private IQueryable<T> ApplySpecification(ISpecification<T, Tkey> Spec)
        {
            return SpecificatoinEvalutor<T, Tkey>.CreatQuery(_dbSet, Spec);
        }

        public async Task<T> GetByIdWithSpecification(ISpecification<T, Tkey> Spec)
        {
            return await ApplySpecification(Spec).FirstOrDefaultAsync();
        }

        public async Task<T> GetById(Tkey id)
        {
            return await _dbSet.Where(c => c.Id.Equals(id) && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public IQueryable<T> GetByCriteria(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).Where(e => !e.IsDeleted);
        }
        /*      public async Task DeleteAsync(Tkey id)
              {
                  var entity = await GetByIdAsync(id);
                  if (entity != null)
                  {
                      entity.IsDeleted = true;
                      await UpdatePartialAsync(entity);
                  }
              }*/
        public async Task<bool> IsExistsAsync(Tkey id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id) && !e.IsDeleted) is not null ? true : false;

        }


        public async Task<int> ExecuteUpdateAsync<TProperty>(
           Tkey id,
           Func<T, TProperty> propertySelector,
           TProperty newValue)
        {
            if (id == null)
                return 0;

            var rowsAffected = await _dbSet
                .Where(e => e.Id!.Equals(id) && !e.IsDeleted)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(propertySelector, _ => newValue));

            return rowsAffected;
        }

        public async Task AddCollectionAsync(ICollection<T> eq)
        {

            await _dbSet.AddRangeAsync(eq);
        }

        public async Task<bool> DeleteAsync(Tkey id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            var result = await ExecuteUpdateAsync(id, t => t.IsDeleted, true);
            return result > 0;
        }

    }
}
