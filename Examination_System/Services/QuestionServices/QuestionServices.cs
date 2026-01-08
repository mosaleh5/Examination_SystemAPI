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
            if (!validationResult.IsValid) {
                return Result<QuestionToReturnDto>.ValidaitonFailure(validationResult);
            }
           
            if (!await _courseServices.IsExistsAsync(createQuestionDto.CourseId))
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {createQuestionDto.CourseId} not found");
            }

            var question = _mapper.Map<CreateQuestionDto, Question>(createQuestionDto);
            await _unitOfWork.Repository<Question, int>().AddAsync(question);
            
          
            var rowsAffected = await _unitOfWork.CompleteAsync();
            var RowEffected = await _unitOfWork.CompleteAsync();
            if (RowEffected != null || RowEffected < 1)
            {
                return Result<QuestionToReturnDto>.Failure(
                ErrorCode.DatabaseError,
                $"Error Happened  in Database");

            }

            var questionToReturnDto = _mapper.Map<Question, QuestionToReturnDto>(question);
            return Result<QuestionToReturnDto>.Success(questionToReturnDto);
        }

        public async Task<Result<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructorAndCourseAsync(
            string? instructorId, 
            int? courseId)
        {
            if (string.IsNullOrWhiteSpace(instructorId))
            {
                return Result<IEnumerable<QuestionToReturnDto>>.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }

            if (!courseId.HasValue || courseId.Value <= 0)
            {
                return Result<IEnumerable<QuestionToReturnDto>>.Failure(
                    ErrorCode.ValidationError,
                    "Valid course ID is required");
            }

            var questionsSpec = new QuestionSpecifications(
                q => q.InstructorId == instructorId && q.CourseId == courseId);
            
            var questions = _unitOfWork.Repository<Question, int>()
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

            var result = Result<IEnumerable<QuestionToReturnDto>>.Success(questionToReturnDto);
            return result;
        }

        public async Task<Result<IEnumerable<QuestionToReturnDto>>> GetQuestionsByInstructorAsync(string? instructorId)
        {
            if (string.IsNullOrWhiteSpace(instructorId))
            {
                return Result<IEnumerable<QuestionToReturnDto>>.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }

            var questionsSpec = new QuestionSpecifications(q => q.InstructorId == instructorId);
            var questions = _unitOfWork.Repository<Question, int>()
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

            var result = Result<IEnumerable<QuestionToReturnDto>>.Success(questionToReturnDto);
            return result;
        }

        public async Task<Result<QuestionToReturnDto>> GetQuestionByIdAsync(int questionId, string instructorId)
        {
            if (questionId <= 0)
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.ValidationError,
                    "Valid question ID is required");
            }

            if (string.IsNullOrWhiteSpace(instructorId))
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }

            var questionSpec = new QuestionSpecifications(
                q => q.Id == questionId && q.InstructorId == instructorId);
            
            var question = await _unitOfWork.Repository<Question, int>()
                .GetByIdWithSpecification(questionSpec);

            if (question == null)
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.NotFound,
                    $"Question with ID {questionId} not found or you don't have permission to access it");
            }

            var questionToReturnDto = _mapper.Map<Question, QuestionToReturnDto>(question);
            var result = Result<QuestionToReturnDto>.Success(questionToReturnDto);
            return result;
        }

        public async Task<Result<QuestionToReturnDto>> UpdateQuestionAsync(UpdateQuestionDto updateQuestionDto)
        {
            // Validate input
            var validationResult = await _updateQuestionValidator.ValidateAsync(updateQuestionDto);
            if (!validationResult.IsValid)
            {
                return Result<QuestionToReturnDto>.ValidaitonFailure(validationResult);
            }


            var questionSpec = new QuestionSpecifications(
                q => q.Id == updateQuestionDto.Id && q.InstructorId == updateQuestionDto.InstructorId);
            
            var existingQuestion = await _unitOfWork.Repository<Question, int>()
                .GetByIdWithSpecification(questionSpec);

            if (existingQuestion == null)
            {
                return Result<QuestionToReturnDto>.Failure(
                    ErrorCode.NotFound,
                    $"Question with ID {updateQuestionDto.Id} not found or you don't have permission to update it");
            }
            
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
                var choice = _mapper.Map<Choice>(choiceDto);
                choice.QuestionId = existingQuestion.Id;
                existingQuestion.Choices.Add(choice);
            }

            await _unitOfWork.Repository<Question, int>().UpdatePartialAsync(existingQuestion);
            var RowEffected =   await _unitOfWork.CompleteAsync();
            if (RowEffected != null || RowEffected < 1) 
            {
                return Result<QuestionToReturnDto>.Failure(
                ErrorCode.DatabaseError,
                $"Error Happened  in Database");

            }
            // Retrieve updated question
            var updatedQuestionSpec = new QuestionSpecifications(q => q.Id == existingQuestion.Id);
            var updatedQuestion = await _unitOfWork.Repository<Question, int>()
                .GetByIdWithSpecification(updatedQuestionSpec);

            var questionToReturnDto = _mapper.Map<Question, QuestionToReturnDto>(updatedQuestion);
            var result = Result<QuestionToReturnDto>.Success(questionToReturnDto);
            return result;  
        }

        public async Task<Result> DeleteQuestionAsync(int questionId, string instructorId)
        {
            if (questionId <= 0)
            {
                return Result.Failure(
                    ErrorCode.ValidationError,
                    "Valid question ID is required");
            }

            if (string.IsNullOrWhiteSpace(instructorId))
            {
                return Result.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }

      
            var questionSpec = new QuestionSpecifications(
                q => q.Id == questionId && q.InstructorId == instructorId);
            
            var question = await _unitOfWork.Repository<Question, int>()
                .GetByIdWithSpecification(questionSpec);

            if (question == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Question with ID {questionId} not found or you don't have permission to delete it");
            }

          
            var examQuestions = await _unitOfWork.Repository<ExamQuestion, int>()
                .GetAll()
                .Where(eq => eq.QuestionId == questionId)
                .ToListAsync();

            if (examQuestions.Any())
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Cannot delete question with ID {questionId} because it is used in {examQuestions.Count} exam(s)");
            }

            // Delete - let middleware handle database exceptions
            await _unitOfWork.Repository<Question, int>().DeleteAsync(questionId);
            var RowEffected = await _unitOfWork.CompleteAsync();
            if (RowEffected != null || RowEffected < 1)
            {
                return Result<QuestionToReturnDto>.Failure(
                ErrorCode.DatabaseError,
                $"Error Happened  in Database");

            }

            var result = Result.Success();
            return result;
        }
    }
}
