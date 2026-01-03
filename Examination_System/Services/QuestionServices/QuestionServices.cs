using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Services.CourseServices;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.QuestionServices
{
    public class QuestionServices : IQuestionServices
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly ICourseServices _courseServices;
        private readonly IMapper _mapper;

        public QuestionServices(IUnitOfWork unitOFWork,
            ICourseServices courseServices,
            IMapper Mapper)
        {
            _unitOfWork = unitOFWork;
            _courseServices = courseServices;
            _mapper = Mapper;
        }

      

        public async Task<QuestionToReturnDto> CreateQuestionAsync (CreateQuestionDto createQuestionDto)
        {
           // var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (!await _courseServices.IsExistsAsync(createQuestionDto.CourseId))
                    throw new KeyNotFoundException($"Course with ID {createQuestionDto.CourseId} not found");
                var question = _mapper.Map<CreateQuestionDto, Question>(createQuestionDto);
                await _unitOfWork.Repository<Question, int>().AddAsync(question);
                await _unitOfWork.CompleteAsync();
               /* var Choices = _unitOfWork.Repository<Choice, int>();
                foreach (var choice in question.Choices)
                {
                    choice.QuestionId = question.Id;
                    await Choices.AddAsync(choice);
                }*/
                //await _unitOfWork.CompleteAsync();
                //await transaction.CommitAsync();
                var questionToReturnDto = _mapper.Map<Question, QuestionToReturnDto>(question);
                return questionToReturnDto;
            }
            catch (Exception e)
            {
               // await transaction.RollbackAsync();
                throw new Exception("can not add new Question" ,e );
            }
       

        }

        public async Task<IEnumerable<QuestionToReturnDto>> GetQuestionsByInstructorAndCourseAsync(string? instructorId, int? CourseId)
        {
            var QuestionsSpecs = new QuestionSpecifications(c=>c.InstructorId ==  instructorId &&  c.CourseId == CourseId);
            var questions = _unitOfWork.Repository<Question, int>().GetAllWithSpecificationAsync(QuestionsSpecs);
      
            var questionToReturnDto =await  questions.ProjectTo<QuestionToReturnDto>(_mapper.ConfigurationProvider).ToListAsync();
            if (questionToReturnDto == null || !questions.Any())
                throw new KeyNotFoundException("No questions found for the given instructor for this course");
            return questionToReturnDto;
        }

        public async Task<IEnumerable<QuestionToReturnDto>> GetQuestionsByInstructorAsync(string? instructorId)
        {
            var QuestionsSpecs = new QuestionSpecifications(c=>c.InstructorId == instructorId);
            var questions = _unitOfWork.Repository<Question, int>().GetAllWithSpecificationAsync(QuestionsSpecs);

            var questionToReturnDto = await questions.ProjectTo<QuestionToReturnDto>(_mapper.ConfigurationProvider).ToListAsync();
            if (questionToReturnDto == null || !questions.Any())
                throw new KeyNotFoundException("No questions found for the given instructor for this course");
            return questionToReturnDto;
        }

        public async Task<QuestionToReturnDto> GetQuestionByIdAsync(int questionId, string instructorId)
        {
            var questionSpec = new QuestionSpecifications(q => q.Id == questionId && q.InstructorId == instructorId);
            var question = await _unitOfWork.Repository<Question, int>().GetByIdWithSpecification(questionSpec);

            if (question == null)
                throw new KeyNotFoundException($"Question with ID {questionId} not found or you don't have permission to access it.");

            return _mapper.Map<Question, QuestionToReturnDto>(question);
        }

        public async Task<QuestionToReturnDto> UpdateQuestionAsync(UpdateQuestionDto updateQuestionDto)
        {
            try
            {
                var questionSpec = new QuestionSpecifications(q => q.Id == updateQuestionDto.Id && q.InstructorId == updateQuestionDto.InstructorId);
                var existingQuestion = await _unitOfWork.Repository<Question, int>().GetByIdWithSpecification(questionSpec);

                if (existingQuestion == null)
                    throw new KeyNotFoundException($"Question with ID {updateQuestionDto.Id} not found or you don't have permission to update it.");

                if (!await _courseServices.IsExistsAsync(updateQuestionDto.CourseId))
                    throw new KeyNotFoundException($"Course with ID {updateQuestionDto.CourseId} not found");

                _mapper.Map(updateQuestionDto, existingQuestion);

                var existingChoiceIds = existingQuestion.Choices.Select(c => c.Id).ToList();
                var choiceRepo = _unitOfWork.Repository<Choice, int>();

                foreach (var existingChoiceId in existingChoiceIds)
                {
                    await choiceRepo.DeleteAsync(existingChoiceId);
                }

                existingQuestion.Choices.Clear();

                foreach (var choiceDto in updateQuestionDto.Choices)
                {
                    var choice = _mapper.Map<ChoiceDto, Choice>(choiceDto);
                    choice.QuestionId = existingQuestion.Id;
                    existingQuestion.Choices.Add(choice);
                }

                await _unitOfWork.Repository<Question, int>().UpdatePartialAsync(existingQuestion);
                await _unitOfWork.CompleteAsync();

                var updatedQuestionSpec = new QuestionSpecifications(q => q.Id == existingQuestion.Id);
                var updatedQuestion = await _unitOfWork.Repository<Question, int>().GetByIdWithSpecification(updatedQuestionSpec);

                return _mapper.Map<Question, QuestionToReturnDto>(updatedQuestion);
            }
            catch (Exception e)
            {
                throw new Exception("Cannot update question", e);
            }
        }

        public async Task<bool> DeleteQuestionAsync(int questionId, string instructorId)
        {
            try
            {
                var questionSpec = new QuestionSpecifications(q => q.Id == questionId && q.InstructorId == instructorId);
                var question = await _unitOfWork.Repository<Question, int>().GetByIdWithSpecification(questionSpec);

                if (question == null)
                    throw new KeyNotFoundException($"Question with ID {questionId} not found or you don't have permission to delete it.");

                var examQuestions = await _unitOfWork.Repository<ExamQuestion, int>().GetAll()
                    .Where(eq => eq.QuestionId == questionId && !eq.IsDeleted)
                    .ToListAsync();

                if (examQuestions.Any())
                {
                    throw new InvalidOperationException($"Cannot delete question with ID {questionId} because it is used in one or more exams.");
                }

                await _unitOfWork.Repository<Question, int>().DeleteAsync(questionId);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Cannot delete question", e);
            }
        }
    }
}
