using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.ExamAttemptServices.Repositories
{
    public class ExamAttemptRepository : IExamAttemptRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamAttemptRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Exam?> GetExamByIdAsync(Guid examId)
        {
            var examSpecifications = new ExamSpecifications(e => e.Id == examId);
            return await _unitOfWork.Repository<Exam>()
                .GetAllWithSpecificationAsync(examSpecifications)
                .FirstOrDefaultAsync();
        }

        public async Task<ExamAttempt?> GetAttemptWithDetailsAsync(Guid attemptId)
        {
            var attemptSpec = new ExamAttemptSpecifications(s => s.Id == attemptId);
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Guid>> GetInstructorExamIdsAsync(Guid instructorId)
        {
            var instructorExamsSpec = new ExamSpecifications(e => e.InstructorId == instructorId);
            return await _unitOfWork.Repository<Exam>()
                .GetAllWithSpecificationAsync(instructorExamsSpec)
                .Select(e => e.Id)
                .ToListAsync();
        }

        public async Task<List<ExamAttemptDto>> GetAttemptsByExamIdsAsync(List<Guid> examIds)
        {
            var attemptSpec = new ExamAttemptSpecifications();
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => examIds.Contains(s.ExamId))
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<ExamAttemptDto>> GetAttemptsByExamIdsAndStudentAsync(List<Guid> examIds, Guid studentId)
        {
            var attemptSpec = new ExamAttemptSpecifications();
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => examIds.Contains(s.ExamId) && s.StudentId == studentId)
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<ExamAttemptDto>> GetAttemptsByStudentIdAsync(Guid studentId)
        {
            var attemptSpec = new ExamAttemptSpecifications();
            return await _unitOfWork.Repository<ExamAttempt>()
                .GetAllWithSpecificationAsync(attemptSpec)
                .Where(s => s.StudentId == studentId)
                .ProjectTo<ExamAttemptDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<Question>> GetQuestionsByIdsAsync(List<Guid> questionIds)
        {
            var questionSpecifications = new QuestionSpecifications(q => questionIds.Contains(q.Id));
            return await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(questionSpecifications)
                .ToListAsync();
        }

        public async Task<bool> IsStudentEnrolledInExamAsync(Guid examId, Guid studentId)
        {
            return await _unitOfWork.Repository<ExamAssignment>()
                .GetAll()
                .AnyAsync(e => e.ExamId == examId && e.StudentId == studentId);
        }
    }
}