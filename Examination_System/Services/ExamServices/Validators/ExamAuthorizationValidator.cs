using Examination_System.Common;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.ExamServices.Validators
{
    public class ExamAuthorizationValidator
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamAuthorizationValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> ValidateCourseExistsAsync(Guid courseId)
        {
            if (!await _unitOfWork.Repository<Course>().IsExistsAsync(courseId))
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {courseId} not found");
            }
            return Result.Success();
        }

        public async Task<Result> ValidateInstructorOfCourseAsync(Guid courseId, Guid instructorId)
        {
            var isInstructor = await _unitOfWork.Repository<Course>()
                .IsExistsByCriteriaAsync(c => c.Id == courseId && c.InstructorId == instructorId);

            if (!isInstructor)
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You are not the instructor of this course");
            }
            return Result.Success();
        }

        public async Task<Result> ValidateInstructorOfExamAsync(Guid examId, Guid instructorId)
        {
            var isInstructor = await _unitOfWork.Repository<Exam>()
                .IsExistsByCriteriaAsync(e => e.Id == examId && e.InstructorId == instructorId);

            if (!isInstructor)
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to modify this exam");
            }
            return Result.Success();
        }

        public async Task<Result<Exam>> ValidateAndGetExamAsync(Guid examId, Guid instructorId)
        {
            var examSpecification = new ExamSpecifications(e => e.Id == examId);
            var exam = await _unitOfWork.Repository<Exam>().GetAllWithSpecificationAsync(examSpecification)
                .FirstOrDefaultAsync();
            if (exam == null)
            {
                return Result<Exam>.Failure(
                    ErrorCode.NotFound,
                    $"Exam with ID {examId:guid} not found");
            }

            if (exam.InstructorId != instructorId)
            {
                return Result<Exam>.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to modify this exam");
            }

            return Result<Exam>.Success(exam);
        }

        public Result ValidateExamNotActive(Exam exam)
        {
            if (exam.IsActive)
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Cannot modify an active exam {exam.Id}");
            }
            return Result.Success();
        }

        public async Task<Result> ValidateStudentEnrolledInCourseAsync(Guid courseId, Guid studentId)
        {
            var isEnrolled = await _unitOfWork.Repository<CourseEnrollment>()
                .GetAll()
                .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (!isEnrolled)
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    $"Student {studentId} is not enrolled in course {courseId}. Student must be enrolled in the course first");
            }
            return Result.Success();
        }

        public async Task<Result> ValidateStudentNotEnrolledInExamAsync(Guid examId, Guid studentId)
        {
            var isAlreadyEnrolled = await _unitOfWork.Repository<ExamAssignment>()
                .GetAll()
                .AnyAsync(e => e.ExamId == examId && e.StudentId == studentId);

            if (isAlreadyEnrolled)
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Student {studentId} is already assigned to exam {examId:guid}");
            }
            return Result.Success();
        }
    }
}