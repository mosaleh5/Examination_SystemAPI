using AutoMapper;
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;

namespace Examination_System.Services
{
    public class GenericServices<T> : IGenericServices<T> where T : BaseModelGuid
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper = null!;

        public GenericServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public GenericServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> IsExistsAsync(Guid Id)
        {
            var exists = await _unitOfWork.Repository<T>().IsExistsAsync(Id);
            return exists;
        }
    }
}
