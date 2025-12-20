using Examination_System.DTOs.Course;
using Examination_System.Mappers;
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;

namespace Examination_System.Services
{
    public class CourseServices
    {
        private readonly UnitOfWork _unitOfWork;

        public CourseServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            var courseSpec = new CourseSpecifications();
            var courses = await _unitOfWork.Repository<Course>().GetAllWithSpecificationAsync(courseSpec);
            return CourseMapper.ToDtoList(courses);
        }

        public async Task<CourseDetailsDto> GetByIdAsync(int id)
        {
            var spec = new CourseSpecifications(id);
            var course = await _unitOfWork.Repository<Course>().GetByIdWithSpecification(spec);
            return CourseMapper.ToDetailsDto(course);
        }

        public async Task<CourseDto> GetByIdWithSpecificationAsync(CourseSpecifications courseSpec)
        {
            var course = await _unitOfWork.Repository<Course>().GetByIdWithSpecification(courseSpec);
            return CourseMapper.ToDto(course);
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            // Validate instructor exists
            var instructor = await _unitOfWork.Repository<Instructor>().GetById(createDto.InstructorId);
            if (instructor == null)
                throw new InvalidOperationException($"Instructor with ID {createDto.InstructorId} not found");

            var course = CourseMapper.ToEntity(createDto);
            await _unitOfWork.Repository<Course>().Add(course);

            // Reload with instructor data
            var spec = new CourseSpecifications(course.ID);
            var courseWithDetails = await _unitOfWork.Repository<Course>().GetByIdWithSpecification(spec);
            
            return CourseMapper.ToDto(courseWithDetails);
        }

        public async Task<CourseDto> UpdateAsync(int id, UpdateCourseDto updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (id != updateDto.ID)
                throw new ArgumentException("ID mismatch");

            var course = await _unitOfWork.Repository<Course>().GetById(id);
            if (course == null)
                throw new InvalidOperationException($"Course with ID {id} not found");

            // Validate instructor exists if being updated
            if (updateDto.InstructorId.HasValue && updateDto.InstructorId.Value > 0)
            {
                var instructor = await _unitOfWork.Repository<Instructor>().GetById(updateDto.InstructorId.Value);
                if (instructor == null)
                    throw new InvalidOperationException($"Instructor with ID {updateDto.InstructorId.Value} not found");
            }

            CourseMapper.UpdateEntity(course, updateDto);
            var updatedCourse = await _unitOfWork.Repository<Course>().UpdateAsync(course);

            // Reload with instructor data
            var spec = new CourseSpecifications(updatedCourse.ID);
            var courseWithDetails = await _unitOfWork.Repository<Course>().GetByIdWithSpecification(spec);
            
            return CourseMapper.ToDto(courseWithDetails);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var course = await _unitOfWork.Repository<Course>().GetById(id);
            if (course == null)
                return false;

            // Soft delete
            course.IsDeleted = true;
            await _unitOfWork.Repository<Course>().UpdateAsync(course);

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var course = await _unitOfWork.Repository<Course>().GetById(id);
            return course != null && !course.IsDeleted;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(int instructorId)
        {
            var spec = new CourseSpecifications();
            var courses = await _unitOfWork.Repository<Course>().GetAllWithSpecificationAsync(spec);
            
            // Filter by instructor in memory
            var filteredCourses = courses.Where(c => c.InstructorId == instructorId);
            return CourseMapper.ToDtoList(filteredCourses);
        }

        public async Task<int> GetEnrolledStudentsCountAsync(int courseId)
        {
            var spec = new CourseSpecifications(courseId);
            var course = await _unitOfWork.Repository<Course>().GetByIdWithSpecification(spec);
            return course?.CourseEnrollments?.Count ?? 0;
        }
    }
}
