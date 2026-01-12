using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.Common;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Services.CourseServices;
using Examination_System.Specifications.SpecsForEntity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.QuestionServices
{
    public class QuestionServices : IQuestionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseServices _courseServices;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateQuestionDto> _createQuestionValidator;
        private readonly IValidator<UpdateQuestionDto> _updateQuestionValidator;

        public QuestionServices(
            IUnitOfWork unitOfWork,
            ICourseServices courseServices,
            IMapper mapper,
            IValidator<CreateQuestionDto> createQuestionValidator,
            IValidator<UpdateQuestionDto> updateQuestionValidator)
        {
            _unitOfWork = unitOfWork;
            _courseServices = courseServices;
            _mapper = mapper;
            _createQuestionValidator = createQuestionValidator;
            _updateQuestionValidator = updateQuestionValidator;
        }

        public async Task<Result<QuestionToReturnDto>> CreateQuestionAsync(CreateQuestionDto createQuestionDto)
        {
            var validationResult = await _createQuestionValidator.ValidateAsync(createQuestionDto);
            if (!validationResult.IsValid)
            {
                return Result<QuestionToReturnDto>.ValidaitonFailure(validationResult);
            }
           
            if (!await _courseServices.IsExistsAsync(createQuestionDto.CourseId))
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {createQuestionDto.CourseId} not found");
            }

            var question = _mapper.Map<CreateQuestionDto, Question>(createQuestionDto);
            await _unitOfWork.Repository<Question>().AddAsync(question);
            
            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to create question. Database error occurred.");
            }

            var questionToReturnDto = _mapper.Map<Question, QuestionToReturnDto>(question);
            return Result<QuestionToReturnDto>.Success(questionToReturnDto);
        }

        public async Task<Result<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructorAndCourseAsync(
            Guid? instructorId,
            Guid? courseId)
        {
            var questionsSpec = new QuestionSpecifications(
                q => q.InstructorId == instructorId && q.CourseId == courseId);
            
            var questions = _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(questionsSpec);

            var questionToReturnDto = await questions
                .ProjectTo<QuestionToReturnDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!questionToReturnDto.Any())
            {
                return Result<IEnumerable<QuestionToReturnDto>>.Failure(
                    ErrorCode.NotFound,
                    $"No questions found for instructor {instructorId} in course {courseId}");
            }

            return Result<IEnumerable<QuestionToReturnDto>>.Success(questionToReturnDto);
        }

        public async Task<Result<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructorAsync(Guid? instructorId)
        {
            var questionsSpec = new QuestionSpecifications(q => q.InstructorId == instructorId);
            var questions = _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(questionsSpec);

            var questionToReturnDto = await questions
                .ProjectTo<QuestionToReturnDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!questionToReturnDto.Any())
            {
                return Result<IEnumerable<QuestionToReturnDto>>.Failure(
                    ErrorCode.NotFound,
                    $"No questions found for instructor {instructorId}");
            }

            return Result<IEnumerable<QuestionToReturnDto>>.Success(questionToReturnDto);
        }

        public async Task<Result<QuestionToReturnDto>> GetQuestionByIdAsync(Guid questionId, Guid instructorId)
        {
            var questionSpec = new QuestionSpecifications(
                q => q.Id == questionId && q.InstructorId == instructorId);
            
            var question = await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(questionSpec)
                .FirstOrDefaultAsync();

            if (question == null)
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.NotFound,
                    $"Question with ID {questionId} not found or you don't have permission to access it");
            }

            var questionToReturnDto = _mapper.Map<Question, QuestionToReturnDto>(question);
            return Result<QuestionToReturnDto>.Success(questionToReturnDto);
        }

        public async Task<Result<QuestionToReturnDto>> UpdateQuestionAsync(UpdateQuestionDto updateQuestionDto)
        {
            var validationResult = await _updateQuestionValidator.ValidateAsync(updateQuestionDto);
            if (!validationResult.IsValid)
            {
                return Result<QuestionToReturnDto>.ValidaitonFailure(validationResult);
            }

            var questionSpec = new QuestionSpecifications(
                q => q.Id == updateQuestionDto.Id && q.InstructorId == updateQuestionDto.InstructorId);
            
            var existingQuestion = await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(questionSpec)
                .FirstOrDefaultAsync();

            if (existingQuestion == null)
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.NotFound,
                    $"Question with ID {updateQuestionDto.Id} not found or you don't have permission to update it");
            }
            
            _mapper.Map(updateQuestionDto, existingQuestion);

            var existingChoiceIds = existingQuestion.Choices.Select(c => c.Id).ToList();
            var choiceRepo = _unitOfWork.Repository<Choice>();

            foreach (var existingChoiceId in existingChoiceIds)
            {
                await choiceRepo.DeleteAsync(existingChoiceId);
            }

            existingQuestion.Choices.Clear();

            foreach (var choiceDto in updateQuestionDto.Choices)
            {
                var choice = _mapper.Map<Choice>(choiceDto);
                choice.QuestionId = existingQuestion.Id;
                existingQuestion.Choices.Add(choice);
            }

            await _unitOfWork.Repository<Question>().UpdatePartialAsync(existingQuestion);
            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to update question. Database error occurred.");
            }

            var updatedQuestionSpec = new QuestionSpecifications(q => q.Id == existingQuestion.Id);
            var updatedQuestion = await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(updatedQuestionSpec)
                .FirstOrDefaultAsync();

            var questionToReturnDto = _mapper.Map<Question, QuestionToReturnDto>(updatedQuestion);
            return Result<QuestionToReturnDto>.Success(questionToReturnDto);
        }

        public async Task<Result> DeleteQuestionAsync(Guid questionId, Guid? instructorId)
        {
            var questionSpec = new QuestionSpecifications(
                q => q.Id == questionId && q.InstructorId == instructorId);
            
            var question = await _unitOfWork.Repository<Question>()
                .GetAllWithSpecificationAsync(questionSpec)
                .FirstOrDefaultAsync();

            if (question == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Question with ID {questionId} not found or you don't have permission to delete it");
            }

            var examQuestions = await _unitOfWork.Repository<ExamQuestion>()
                .GetAll()
                .Where(eq => eq.QuestionId == questionId)
                .ToListAsync();

            if (examQuestions.Any())
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Cannot delete question with ID {questionId} because it is used in {examQuestions.Count} exam(s)");
            }

            await _unitOfWork.Repository<Question>().DeleteAsync(questionId);
            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to delete question. Database error occurred.");
            }

            return Result.Success();
        }
    }
}
