using Examination_System.Models;

namespace Examination_System.Services
{
    public interface IGenericServices<T> where T : BaseModelGuid
    {
        Task<bool> IsExistsAsync(Guid Id);
    }
}
