using System.Reflection.Metadata.Ecma335;

namespace Examination_System.Services.StudentService.Repository
{
    public interface IStudentRepository
    {
        public Task<bool> IsExists(Guid StudentId);
    }
}
