using AutoMapper;
using Examination_System.Common;
using Examination_System.Common.Constants;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Services.CourseServices;
using Examination_System.Services.QuestionServices.Repositories;
using FluentValidation;

namespace Examination_System.Services.QuestionServices
{
    /// <summary>
    /// Service for managing questions and their choices
    /// </summary>
    public class QuestionServices : IQuestionServices
    {
        private readonly IQuestionRepository _repository;
        private readonly ICourseServices _courseServices;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateQuestionDto> _createValidator;
        private readonly IValidator<UpdateQuestionDto> _updateValidator;

        public QuestionServices(
            IQuestionRepository repository,
            ICourseServices courseServices,
            IMapper mapper,
            IValidator<CreateQuestionDto> createValidator,
            IValidator<UpdateQuestionDto> updateValidator)
        {
            _repository = repository;
            _courseServices = courseServices;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        #region Public Methods

        public async Task<Result<QuestionToReturnDto>> CreateAsync(CreateQuestionDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return Result<QuestionToReturnDto>.ValidaitonFailure(validationResult);

            if (!await _courseServices.IsExistsAsync(dto.CourseId))
                return Failure(QuestionErrorMessages.CourseNotFound, dto.CourseId);

          
            var question = _mapper.Map<Question>(dto);
            await _repository.AddAsync(question);

            if (await _repository.SaveChangesAsync() < 1)
                return Failure(QuestionErrorMessages.CreateFailed);
            var result = _mapper.Map<QuestionToReturnDto>(question);
            return Success(result);
        }

        public async Task<Result<QuestionToReturnDto>> GetByIdAsync(Guid questionId, Guid instructorId)
        {
            var question = await _repository.GetByIdAndInstructorAsync(questionId, instructorId);
            
            if (question is null)
                return Failure(QuestionErrorMessages.NotFoundOrNoPermission, questionId);

            return Success(_mapper.Map<QuestionToReturnDto>(question));
        }

        public async Task<Result<IEnumerable<QuestionToReturnDto>>> GetByInstructorAsync(Guid instructorId)
        {
            var questions = await _repository.GetByInstructorAsync(instructorId);

            if (questions.Count == 0)
                return Result<IEnumerable<QuestionToReturnDto>>.Failure(
                    ErrorCode.NotFound,
                    string.Format(QuestionErrorMessages.NoQuestionsForInstructor, instructorId));

            return Result<IEnumerable<QuestionToReturnDto>>.Success(questions);
        }

        public async Task<Result<IEnumerable<QuestionToReturnDto>>> GetByInstructorAndCourseAsync(
            Guid instructorId, 
            Guid courseId)
        {
            var questions = await _repository.GetByInstructorAndCourseAsync(instructorId, courseId);

            if (questions.Count == 0)
                return Result<IEnumerable<QuestionToReturnDto>>.Failure(
                    ErrorCode.NotFound,
                    string.Format(QuestionErrorMessages.NoQuestionsForInstructorInCourse, instructorId, courseId));

            return Result<IEnumerable<QuestionToReturnDto>>.Success(questions);
        }

        public async Task<Result<QuestionToReturnDto>> UpdateAsync(UpdateQuestionDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return Result<QuestionToReturnDto>.ValidaitonFailure(validationResult);

            var question = await _repository.GetByIdAndInstructorAsync(dto.Id, dto.InstructorId);
            if (question is null)
                return Failure(QuestionErrorMessages.NotFoundOrNoPermission, dto.Id);

            _mapper.Map(dto, question);

           
            await ReplaceChoicesAsync(question, dto.Choices);

        
            await _repository.UpdateAsync(question);
            if (await _repository.SaveChangesAsync() < 1)
                return Failure(QuestionErrorMessages.UpdateFailed);

           
            var updatedQuestion = await _repository.GetByIdAsync(question.Id);
            var result = _mapper.Map<QuestionToReturnDto>(updatedQuestion);
            return Success(result);
        }

        public async Task<Result> DeleteAsync(Guid questionId, Guid instructorId)
        {
            
            var question = await _repository.GetByIdAndInstructorAsync(questionId, instructorId);
            if (question is null)
                return Result.Failure(
                    ErrorCode.NotFound,
                    string.Format(QuestionErrorMessages.NotFoundOrNoPermission, questionId));

            
            var examUsageCount = await _repository.GetExamUsageCountAsync(questionId);
            if (examUsageCount > 0)
                return Result.Failure(
                    ErrorCode.Conflict,
                    string.Format(QuestionErrorMessages.UsedInExams, questionId, examUsageCount));

           
             var result = await _repository.DeleteAsync(questionId);
           
            return result ? Result.Success():Result.Failure(ErrorCode.QuestionNotDeleted , QuestionErrorMessages.DeleteFailed);
        }

        #endregion

        #region Private Helper Methods

        private async Task ReplaceChoicesAsync(Question question, IEnumerable<UpdateChoiceDto> newChoices)
        {
         
            var existingChoiceIds = question.Choices.Select(c => c.Id).ToList();
            await _repository.DeleteChoicesAsync(existingChoiceIds);
            question.Choices.Clear();

         
            foreach (var choiceDto in newChoices)
            {
                var choice = _mapper.Map<Choice>(choiceDto);
                choice.QuestionId = question.Id;
                question.Choices.Add(choice);
            }
        }

        private static Result<QuestionToReturnDto> Failure(string messageFormat, params object[] args)
            => Result<QuestionToReturnDto>.Failure(
                ErrorCode.NotFound,
                string.Format(messageFormat, args));

        private static Result<QuestionToReturnDto> Success(QuestionToReturnDto data)
            => Result<QuestionToReturnDto>.Success(data);

        #endregion
    }
}
