using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.DTOs.Course;
using Examination_System.Mappers;

//using Examination_System.Mappers;
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.CourseServices
{
    public class CourseServices : GenericServices<Course , int> , ICourseServices
    {
        public CourseServices(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper)
        {

        }

        // 
        /*    public async Task<IEnumerable<CourseDto>> GetAllAsync()
            {
                var courseSpec = new CourseSpecifications();
                var courses = await _unitOfWork.Repository<Course, int>().GetAllWithSpecificationAsync(courseSpec);
                return CourseMapper.ToDtoList(courses);
            }*/

        public async Task<IEnumerable<CourseDtoToReturn>> GetAllForInstructorAsync(string InstructorId)
        {
            var courseSpec = new CourseSpecifications(c=>c.InstructorId == InstructorId);
            var courses = _unitOfWork.Repository<Course, int>().GetAllWithSpecificationAsync(courseSpec);
            var CourseDetailsDto = await courses.ProjectTo<CourseDtoToReturn>(_mapper.ConfigurationProvider).ToListAsync();
            return CourseDetailsDto;
        }
        public async Task<CourseDtoToReturn> CreateAsync(CreateCourseDto createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            var course = _mapper.Map<CreateCourseDto, Course>(createDto);

            await _unitOfWork.Repository<Course, int>().Add(course);
            await _unitOfWork.SaveChangesAsync();
            
            
            var courseToReturn = _mapper.Map<Course, CourseDtoToReturn>(course);

            return courseToReturn;
                     
        }
        public async Task<CourseDetailsDto> GetByIdAsync(int id)
        {
            var spec = new CourseSpecifications(c => c.Id == id);
            var course = await _unitOfWork.Repository<Course,int>().GetByIdWithSpecification(spec);
         
            return CourseMapper.ToDetailsDto(course);
        }
     

        public  async Task<bool> IsInstructorOfCourseAsync(int courseId, string instructorId)
        {
            var result = await _unitOfWork.Repository<Course, int>().GetAll()
                .AnyAsync(c => c.Id == courseId && c.InstructorId == instructorId);
            return result;

        }



        public async Task<bool> EnrollStudentInCourseAsync(int courseId, string studentId)
        {
            if (!await _unitOfWork.Repository<Course, int>().IsExistsAsync(courseId))
            {
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");
            }
            if (!await _unitOfWork.Repository<Student, string>().IsExistsAsync(studentId))
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found.");
            }
            if (await IsStudentAlreadyEnrolledAsync(courseId, studentId))
            {
                throw new InvalidOperationException($"Student {studentId} is already enrolled in course {courseId}.");
            }
            var enrollmentCourse = new CourseEnrollment
            {
                CourseId = courseId,
                StudentId = studentId,
                EnrollmentDate = DateTime.UtcNow
            };
            await _unitOfWork.Repository<CourseEnrollment, int>().Add(enrollmentCourse);
              var result  = await _unitOfWork.SaveChangesAsync() > 0;
            return result;
        }

        private async Task<bool> IsStudentAlreadyEnrolledAsync(int courseId, string studentId)
        {
            return await _unitOfWork.Repository<CourseEnrollment, int>().GetAll()
                .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        }

        public async Task<CourseDtoToReturn> UpdateAsync(UpdateCourseDto updateCourseDto, string userId)
        {
            if (updateCourseDto == null)
                throw new ArgumentNullException(nameof(updateCourseDto));
            if(!await IsInstructorOfCourseAsync(updateCourseDto.ID , userId))
                throw new UnauthorizedAccessException("You are not authorized to update this course.");
            var course = _mapper.Map< Course>(updateCourseDto);
            await _unitOfWork.Repository<Course, int>().UpdatePartialAsync(course);
            await _unitOfWork.SaveChangesAsync();

            var courseToReturn = _mapper.Map<Course, CourseDtoToReturn>(course);

            return courseToReturn;
        }
        public async Task DeleteAsync(int courseId)
        {
            var course = await IsExistsAsync(courseId);
            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");
            }
            await _unitOfWork.Repository<Course, int>().DeleteAsync(courseId);
            await _unitOfWork.CompleteAsync();
        }

    }
}
