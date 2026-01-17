using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.CourseServices.Repository
{
    public class CourseRepository : ICourseRepository
    {

        private readonly IUnitOfWork _unitOfWork;

        public CourseRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<bool> ExistsAsync(Guid courseId)
                => await _unitOfWork.Repository<Course>().IsExistsAsync(courseId);

            public async Task<bool> IsInstructorAsync(Guid courseId, Guid instructorId)
                => await _unitOfWork.Repository<Course>()
                    .IsExistsByCriteriaAsync(c => c.Id == courseId && c.InstructorId == instructorId);

             public async Task<Course?> GetByIdAsync(Guid courseId)
                { 
            
                return await _unitOfWork.Repository<Course>().GetAllWithSpecificationAsync(new CourseSpecifications(c => c.Id == courseId))
                    .FirstOrDefaultAsync();
                 }
            public IQueryable<Course> GetByInstructor(Guid instructorId)
            {
                 var courseSpecs = new CourseSpecifications(c => c.InstructorId == instructorId);
                  return _unitOfWork.Repository<Course>().GetAllWithSpecificationAsync(courseSpecs);

        }
        public async Task<bool> IsStudentAlreadyEnrolledAsync(Guid courseId, Guid studentId)
        {
            return await _unitOfWork.Repository<CourseEnrollment>().GetAll()
          .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        }
        public async Task<bool> IsInstructorOfCourseAsync(Guid courseId, Guid instructorId)
        {
            return await _unitOfWork.Repository<Course>().GetAll()
                .AnyAsync(c => c.Id == courseId && c.InstructorId == instructorId);
        }

    }
}
