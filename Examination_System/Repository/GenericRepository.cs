using System.Linq.Expressions;
using Examination_System.Data;
using Examination_System.Models;
using Examination_System.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Examination_System.Repository
{
    // For entities with int IDs
    public class GenericRepository<T> : IGenericRepository<T> where T :  BaseModelGuid
    {
        private readonly Context _context;
        private  DbSet<T> _dbSet;
     
        public GenericRepository(Context context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var res = await _dbSet.Where(c => c.Id.Equals(id) && !c.IsDeleted).FirstOrDefaultAsync();
            return res!;
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
        public IQueryable<T> GetAllWithSpecificationAsync(ISpecification<T> specifications)
        {
            return ApplySpecification(specifications).AsQueryable<T>();
        }
        private IQueryable<T> ApplySpecification(ISpecification<T> Spec)
        {
            return SpecificatoinEvalutor<T>.CreatQuery(_dbSet, Spec);
        }

        public  IQueryable<T> GetByIdWithSpecification(ISpecification<T> Spec)
        {
            return  ApplySpecification(Spec);
        }

        public async Task<T> GetById(Guid id)
        {
            return await _dbSet.Where(c => c.Id.Equals(id) && !c.IsDeleted).FirstOrDefaultAsync()!;
        }

        public IQueryable<T> GetByCriteria(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).Where(e => !e.IsDeleted);
        }
        /*      public async Task DeleteAsync(Guid id)
              {
                  var entity = await GetByIdAsync(id);
                  if (entity != null)
                  {
                      entity.IsDeleted = true;
                      await UpdatePartialAsync(entity);
                  }
              }*/
        public async Task<bool> IsExistsAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id) && !e.IsDeleted) is not null ? true : false;

        }

        public async Task<bool> IsExistsByCriteriaAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate) is not null ? true : false;
        }
        public async Task<int> ExecuteUpdateAsync<TProperty>(
         Guid id,
         Func<T, TProperty> propertySelector,
         TProperty newValue)
        {
            if (id == Guid.Empty)
                return 0;

            return await _dbSet
                .Where(e => e.Id == id && !e.IsDeleted)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(propertySelector, _ => newValue));
        }



        public async Task AddCollectionAsync(ICollection<T> eq)
        {

            await _dbSet.AddRangeAsync(eq);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            var result = await ExecuteUpdateAsync<T>(t => t.Id == id,
                setters => setters
                .SetProperty(s=>s.IsDeleted , true)
                .SetProperty(s=>s.UpdatedAt , DateTime.UtcNow))
                ;       
            return result > 0;
        }

        public async Task<int> ExecuteUpdateAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls) 
            where TEntity : class
        {
            return await _context.Set<TEntity>()
                .Where(predicate)
                .ExecuteUpdateAsync(setPropertyCalls);
        }
    }
}
