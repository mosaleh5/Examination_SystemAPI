using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.QuestionServices.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Question?> GetByIdAsync(Guid questionId)
        {
            var spec = new QuestionSpecifications(q => q.Id == questionId);
            return await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(spec)
                .FirstOrDefaultAsync();
        }

        public async Task<Question?> GetByIdAndInstructorAsync(Guid questionId, Guid instructorId)
        {
            var spec = new QuestionSpecifications(q => q.Id == questionId && q.InstructorId == instructorId);
            return await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(spec)
                .FirstOrDefaultAsync();
        }

        public async Task<List<QuestionToReturnDto>> GetByInstructorAsync(Guid instructorId)
        {
            var spec = new QuestionSpecifications(q => q.InstructorId == instructorId);
            return await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(spec)
                .ProjectTo<QuestionToReturnDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<QuestionToReturnDto>> GetByInstructorAndCourseAsync(Guid instructorId, Guid courseId)
        {
            var spec = new QuestionSpecifications(q => q.InstructorId == instructorId && q.CourseId == courseId);
            return await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(spec)
                .ProjectTo<QuestionToReturnDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> GetExamUsageCountAsync(Guid questionId)
        {
            return await _unitOfWork.Repository<ExamQuestion>()
                .GetAll()
                .CountAsync(eq => eq.QuestionId == questionId);
        }

        public async Task AddAsync(Question question)
            => await _unitOfWork.Repository<Question>().AddAsync(question);

        public async Task UpdateAsync(Question question)
            => await _unitOfWork.Repository<Question>().UpdatePartialAsync(question);

        public async Task<bool> DeleteAsync(Guid questionId)
            => await _unitOfWork.Repository<Question>().DeleteAsync(questionId);

        public async Task DeleteChoicesAsync(IEnumerable<Guid> choiceIds)
        {
            var choiceRepo = _unitOfWork.Repository<Choice>();
            foreach (var choiceId in choiceIds)
            {
                await choiceRepo.DeleteAsync(choiceId);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            var result = (int) await _unitOfWork.CompleteAsync();
            return result;
        }
    }
    }