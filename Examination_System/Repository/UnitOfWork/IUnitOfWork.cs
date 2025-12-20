using Examination_System.Models;

namespace Examination_System.Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        void Rollback();
        GenericRepository<T> Repository<T>() where T : BaseModel;
    }
}
