using Examination_System.Data;
using Examination_System.Models;
using System.Collections;

namespace Examination_System.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(Hashtable Repositories, Context context )
        {
            this._repositories = Repositories;
            _context = context;
        }

        public Hashtable _repositories { get; }
        public Context _context { get; }


        public Task CommitAsync() 
            => _context.SaveChangesAsync();

        public void Rollback()
            =>_context.Dispose();
        

        public GenericRepository<T> Repository<T>() where T : BaseModel
        {
          if(!_repositories.ContainsKey(typeof(T).Name))
          {
            var repository = new GenericRepository<T>(_context);
            _repositories.Add(typeof(T).Name, repository);
          }
            return _repositories as GenericRepository<T>; 
        }

    }
}
