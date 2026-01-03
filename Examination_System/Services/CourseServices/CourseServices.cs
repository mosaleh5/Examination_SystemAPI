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
        /*


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
}*/
    }
}
