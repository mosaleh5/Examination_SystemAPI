using Examination_System.Models;

namespace Examination_System.Services
{
    public interface IGenericServices<T , Tkey> where T : class , IBaseModel<Tkey>
    {
        Task<bool> IsExistsAsync(Tkey Id);
    }
}
