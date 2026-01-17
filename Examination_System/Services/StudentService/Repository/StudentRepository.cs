using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;

namespace Examination_System.Services.StudentService.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<bool> IsExists(Guid StudentId)
        {
            return await _unitOfWork.Repository<Student>().IsExistsAsync(StudentId);
        }
    }
}
