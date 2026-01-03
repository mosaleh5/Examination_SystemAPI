using AutoMapper;
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;

namespace Examination_System.Services
{
    public class GenericServices<T, Tkey> : IGenericServices<T, Tkey> where T : class, IBaseModel<Tkey>
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public GenericServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public GenericServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> IsExistsAsync(Tkey Id)
        {
            var exists = await _unitOfWork.Repository<T, Tkey>().IsExistsAsync(Id);
            return exists;
        }
    }
}
