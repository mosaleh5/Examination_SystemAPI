using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.Common;
using Examination_System.DTOs.Student;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.StudentService
{
    public class StudentServices : GenericServices<Student>, IStudentServices
    {
 

        public StudentServices(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
          
        }

        public async Task<Result> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId)
        {
           /* if (string.IsNullOrWhiteSpace(studentId))
            {
                return Result.Failure(
                    ErrorCode.ValidationError,
                    "Student ID is required");
            }

            if (courseId <= 0)
            {
                return Result.Failure(
                    ErrorCode.ValidationError,
                    "Valid course ID is required");
            }*/

            var isEnrolled = await _unitOfWork.Repository<CourseEnrollment>()
                .GetAll()
                .AnyAsync(ce => ce.StudentId == studentId && ce.CourseId == courseId);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<CourseEnrollmentDto>>> GetEnrolledCoursesAsync(Guid studentId)
        {
           /* if (string.IsNullOrWhiteSpace(studentId))
            {
                return Result<IEnumerable<CourseEnrollmentDto>>.Failure(
                    ErrorCode.ValidationError,
                    "Student ID is required");
            }*/

            var enrollments = await _unitOfWork.Repository<CourseEnrollment>()
                .GetAll()
                .Where(ce => ce.StudentId == studentId)
                .Include(ce => ce.Course)
                .ProjectTo<CourseEnrollmentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!enrollments.Any())
            {
                return Result<IEnumerable<CourseEnrollmentDto>>.Failure(
                    ErrorCode.NotFound,
                    $"No courses found for student {studentId}");
            }

            return Result<IEnumerable<CourseEnrollmentDto>>.Success(enrollments);
        }

        public async Task<Result<IEnumerable<ExamAssignmentDto>>> GetAssignedExamsAsync(Guid studentId)
        {
          /*  if (string.IsNullOrWhiteSpace(studentId))
            {
                return Result<IEnumerable<ExamAssignmentDto>>.Failure(
                    ErrorCode.ValidationError,
                    "Student ID is required");
            }*/

            var examAssignments = await _unitOfWork.Repository<ExamAssignment>()
                .GetAll()
                .Where(ea => ea.StudentId == studentId)
                .Include(ea => ea.Exam)
                .ProjectTo<ExamAssignmentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!examAssignments.Any())
            {
                return Result<IEnumerable<ExamAssignmentDto>>.Failure(
                    ErrorCode.NotFound,
                    $"No exams found for student {studentId}");
            }

            return Result<IEnumerable<ExamAssignmentDto>>.Success(examAssignments);
        }

        public async Task<Result<StudentDetailsDto>> GetStudentDetailsAsync(Guid studentId)
        {/*
            if (string.IsNullOrWhiteSpace(studentId))
            {
                return Result<StudentDetailsDto>.Failure(
                    ErrorCode.ValidationError,
                    "Student ID is required");
            }
*/
            var student = await _unitOfWork.Repository<Student>()
                .GetAll()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return Result<StudentDetailsDto>.Failure(
                    ErrorCode.NotFound,
                    $"Student with ID {studentId} not found");
            }

            var studentDetails = _mapper.Map<StudentDetailsDto>(student);
            return Result<StudentDetailsDto>.Success(studentDetails);
        }
    }
}