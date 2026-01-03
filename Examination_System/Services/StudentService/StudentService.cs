
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;

namespace Examination_System.Services.StudentService
{
    public class StudentServices :  GenericServices<Student, string> , IStudentServices
    {

        public StudentServices(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            
        }
    
       
        public Task<bool> IsStudentEnrolledInCourseAsync(string studentId, int courseId)
        {
            throw new NotImplementedException();
        }

     
    }

    }
    