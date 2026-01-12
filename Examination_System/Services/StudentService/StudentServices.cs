/*using AutoMapper;
using Examination_System.Common;
using Examination_System.DTOs.Student;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.StudentService
{
    public class StudentServices : GenericServices<Student>, IStudentServices
    {*//**//*
       

        public StudentServices(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper)
        {
           
        }

        public async Task<Result<bool>> IsStudentEnrolledInCourseAsync(Guid studentId, int courseId)
        {
            if (studentId == Guid.Empty)
            {
                return Result<bool>.Failure(
                    ErrorCode.ValidationError,
                    "Valid student ID is required");
            }

            if (courseId <= 0)
            {
                return Result<bool>.Failure(
                    ErrorCode.ValidationError,
                    "Valid course ID is required");
            }

            var isEnrolled = await _unitOfWork.Repository<CourseEnrollment>()
                .GetAll()
                .AnyAsync(ce => ce.StudentId == studentId && ce.CourseId == courseId);

            return Result<bool>.Success(isEnrolled);
        }

        // ... other methods
    }
}*/