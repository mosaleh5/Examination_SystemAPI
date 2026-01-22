using Examination_System.Data;
using Examination_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;

namespace Examination_System.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork( Context context )
        {
            this._repositories = new Hashtable();
            _context = context;
        }

        public Hashtable _repositories { get; }
        public Context _context { get; }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            return _context.Database.BeginTransactionAsync(ct);
        }
     
        public async Task<int?> CompleteAsync() 
            =>await _context.SaveChangesAsync();     
        
        public GenericRepository<T> Repository<T>() where T :BaseModelGuid
        {
          if(!_repositories.ContainsKey(typeof(T).Name))
          {
            var repository = new GenericRepository<T>(_context);
            _repositories.Add(typeof(T).Name, repository);
          }
            return (_repositories[typeof(T).Name] as GenericRepository<T>)!; 
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
           return await _context.SaveChangesAsync(ct);
        }

        ValueTask IAsyncDisposable.DisposeAsync()
      => _context.DisposeAsync();
    }
}
