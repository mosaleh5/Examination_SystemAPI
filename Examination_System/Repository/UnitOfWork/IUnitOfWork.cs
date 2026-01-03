using Examination_System.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Examination_System.Repository.UnitOfWork
{
    public interface IUnitOfWork :IAsyncDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<int?> CompleteAsync();

        GenericRepository<T,Tkey> Repository<T ,Tkey>() where T :class ,  IBaseModel<Tkey>;
    }
}
