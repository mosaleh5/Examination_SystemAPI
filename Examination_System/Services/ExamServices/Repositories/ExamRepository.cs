using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.DTOs.Exam;
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.ExamServices.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Exam> CreateExamAsync(Exam exam)
        {
            await _unitOfWork.Repository<Exam>().AddAsync(exam);
            return exam;
        }

        public async Task<ExamToReturnDto?> GetExamByIdAsync(Guid examId)
        {
            var examSpecifications = new ExamSpecifications(e => e.Id == examId);
            return await _unitOfWork.Repository<Exam>()
                .GetAllWithSpecificationAsync(examSpecifications)
                .ProjectTo<ExamToReturnDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ExamToReturnDto>> GetAllExamsForInstructorAsync(Guid instructorId)
        {
            var examSpecifications = new ExamSpecifications(e => e.InstructorId == instructorId);
            return await _unitOfWork.Repository<Exam>()
                .GetAllWithSpecificationAsync(examSpecifications)
                .ProjectTo<ExamToReturnDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task ActivateExamAsync(Guid examId)
        {
            await _unitOfWork.Repository<Exam>().ExecuteUpdateAsync<Exam>(
                exam => exam.Id == examId,
                setters => setters
                    .SetProperty(e => e.IsActive, true)
                    .SetProperty(e => e.UpdatedAt, DateTime.UtcNow)
            );
        }

        public async Task UpdateExamFullMarkAsync(Guid examId, int fullMark)
        {
            await _unitOfWork.Repository<Exam>().ExecuteUpdateAsync(examId, e => e.Fullmark, fullMark);
        }

        public async Task<bool> IsStudentExistsAsync(Guid studentId)
        {
            return await _unitOfWork.Repository<Student>().IsExistsAsync(studentId);
        }

        public async Task<bool> IsExamExistsAsync(Guid examId)
        {
            return await _unitOfWork.Repository<Exam>().IsExistsAsync(examId);
        }
    }
}