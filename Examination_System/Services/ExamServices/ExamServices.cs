using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.Common;
using Examination_System.DTOs.Exam;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Services.ExamServices.Managers;
using Examination_System.Services.ExamServices.Repositories;
using Examination_System.Services.ExamServices.Validators;
using Examination_System.Specifications.SpecsForEntity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using static Examination_System.Models.Question;

namespace Examination_System.Services.ExamServices
{
    public class ExamServices : GenericServices<Exam>, IExamServices
    {
        private readonly IValidator<CreateExamDto> _createExamValidator;
        private readonly IValidator<CreateAutomaticExamDto> _createAutomaticExamValidator;
        private readonly IValidator<AssignStudentToExamDto> _assignStudentToExamDtoValidator;
        private readonly IValidator<ActivateExamDto> _activateExamDtoValidator;
        private readonly IValidator<AddQuestionsToExamDto> _addQuestionsToExamDtoValidator;
        private readonly IValidator<ReplaceExamQuestionsDto> _replaceExamQuestionsDtoValidator;
        private readonly IValidator<RemoveQuestionFromExamDto> _removeQuestionFromExamDtoValidator;
        private readonly IValidator<GetExamByIdDto> _getExamByIdDtoValidator;
        private readonly IExamRepository _examRepository;
        private readonly ExamAuthorizationValidator _authValidator;
        private readonly ExamQuestionManager _questionManager;

        public ExamServices(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateExamDto> createExamValidator,
            IValidator<CreateAutomaticExamDto> createAutomaticExamValidator,
            IValidator<AssignStudentToExamDto> assignStudentToExamDtoValidator,
            IValidator<ActivateExamDto> activateExamDtoValidator,
            IValidator<AddQuestionsToExamDto> addQuestionsToExamDtoValidator,
            IValidator<ReplaceExamQuestionsDto> replaceExamQuestionsDtoValidator,
            IValidator<RemoveQuestionFromExamDto> removeQuestionFromExamDtoValidator,
            IValidator<GetExamByIdDto> getExamByIdDtoValidator,
            IExamRepository examRepository,
            ExamAuthorizationValidator authValidator,
            ExamQuestionManager questionManager)
            : base(unitOfWork, mapper)
        {
            _createExamValidator = createExamValidator;
            _createAutomaticExamValidator = createAutomaticExamValidator;
            _assignStudentToExamDtoValidator = assignStudentToExamDtoValidator;
            _activateExamDtoValidator = activateExamDtoValidator;
            _addQuestionsToExamDtoValidator = addQuestionsToExamDtoValidator;
            _replaceExamQuestionsDtoValidator = replaceExamQuestionsDtoValidator;
            _removeQuestionFromExamDtoValidator = removeQuestionFromExamDtoValidator;
            _getExamByIdDtoValidator = getExamByIdDtoValidator;
            _examRepository = examRepository;
            _authValidator = authValidator;
            _questionManager = questionManager;
        }

        public async Task<Result<ExamToReturnDto>> CreateExam(CreateExamDto createExamDto)
        {
            var validationResult = await _createExamValidator.ValidateAsync(createExamDto);
            if (!validationResult.IsValid)
                return Result<ExamToReturnDto>.ValidaitonFailure(validationResult);

            var courseValidation = await _authValidator.ValidateCourseExistsAsync(createExamDto.CourseId);
            if (!courseValidation.IsSuccess)
                return Result<ExamToReturnDto>.Failure(courseValidation.Error, courseValidation.ErrorMessage);

            var instructorValidation = await _authValidator.ValidateInstructorOfCourseAsync(createExamDto.CourseId, createExamDto.InstructorId);
            if (!instructorValidation.IsSuccess)
                return Result<ExamToReturnDto>.Failure(instructorValidation.Error, instructorValidation.ErrorMessage);

            var exam = _mapper.Map<Exam>(createExamDto);
            await _examRepository.CreateExamAsync(exam);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
                return Result<ExamToReturnDto>.Failure(ErrorCode.DatabaseError, "Failed to create exam. Database error occurred.");

            var examToReturn = _mapper.Map<ExamToReturnDto>(exam);
            return Result<ExamToReturnDto>.Success(examToReturn);
        }

        public async Task<Result<ExamToReturnDto>> CreateAutomaticExam(CreateAutomaticExamDto createAutomaticExamDto)
        {
            var validationResult = await _createAutomaticExamValidator.ValidateAsync(createAutomaticExamDto);
            if (!validationResult.IsValid)
                return Result<ExamToReturnDto>.ValidaitonFailure(validationResult);

            var courseValidation = await _authValidator.ValidateCourseExistsAsync(createAutomaticExamDto.CourseId);
            if (!courseValidation.IsSuccess)
                return Result<ExamToReturnDto>.Failure(courseValidation.Error, courseValidation.ErrorMessage);

            var instructorValidation = await _authValidator.ValidateInstructorOfCourseAsync(createAutomaticExamDto.CourseId, createAutomaticExamDto.InstructorId);
            if (!instructorValidation.IsSuccess)
                return Result<ExamToReturnDto>.Failure(instructorValidation.Error, instructorValidation.ErrorMessage);

            var exam = _mapper.Map<Exam>(createAutomaticExamDto);
            await _examRepository.CreateExamAsync(exam);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
                return Result<ExamToReturnDto>.Failure(ErrorCode.DatabaseError, "Failed to create automatic exam. Database error occurred.");

            var questionsResult = await _questionManager.SelectBalancedQuestionsAsync(createAutomaticExamDto.CourseId, createAutomaticExamDto.QuestionsCount);
            if (!questionsResult.IsSuccess)
                return Result<ExamToReturnDto>.Failure(questionsResult.Error, questionsResult.ErrorMessage);

            var selectedQuestions = questionsResult.Data!;
            return await AddQuestionsAndFinalizeExamAsync(exam, selectedQuestions, createAutomaticExamDto.InstructorId);
        }

        private async Task<Result<ExamToReturnDto>> AddQuestionsAndFinalizeExamAsync(Exam exam, List<Question> selectedQuestions, Guid instructorId)
        {
            var totalMark = (int)selectedQuestions.Sum(q => q.Mark);
            exam.Fullmark = totalMark;
            await _unitOfWork.Repository<Exam>().UpdatePartialAsync(exam);

            var examQuestions = selectedQuestions.Select(q => new ExamQuestion
            {
                QuestionId = q.Id,
                ExamId = exam.Id
            }).ToList();

            foreach (var eq in examQuestions)
            {
                await _unitOfWork.Repository<ExamQuestion>().AddAsync(eq);
            }

            var examRowsAffected = await _unitOfWork.CompleteAsync();
            if (examRowsAffected < 1)
                return Result<ExamToReturnDto>.Failure(ErrorCode.DatabaseError, "Failed to create automatic exam. Database error occurred.");

            var getExamDto = new GetExamByIdDto
            {
                ExamId = exam.Id,
                InstructorId = instructorId
            };

            var result = await GetExamsForInstructorById(getExamDto);
            if (!result.IsSuccess)
            {
                var examToReturn = _mapper.Map<ExamToReturnDto>(exam);
                return Result<ExamToReturnDto>.Success(examToReturn, "Exam created successfully. Get details using GetById endpoint.");
            }

            return result;
        }

        public async Task<Result<IEnumerable<ExamToReturnDto>>> GetAllExamsForInstructor(Guid? instructorId)
        {
            if (instructorId == null)
                return Result<IEnumerable<ExamToReturnDto>>.Failure(ErrorCode.ValidationError, "Instructor ID is required");

            var exams = await _examRepository.GetAllExamsForInstructorAsync(instructorId.Value);

            if (!exams.Any())
                return Result<IEnumerable<ExamToReturnDto>>.Failure(ErrorCode.NotFound, $"No exams found for instructor {instructorId}");

            return Result<IEnumerable<ExamToReturnDto>>.Success(exams);
        }

        public async Task<Result<ExamToReturnDto>> GetExamsForInstructorById(GetExamByIdDto getExamByIdDto)
        {
            var validationResult = _getExamByIdDtoValidator.Validate(getExamByIdDto);
            if (!validationResult.IsValid)
                return Result<ExamToReturnDto>.ValidaitonFailure(validationResult);

            var authResult = await _authValidator.ValidateInstructorOfExamAsync(getExamByIdDto.ExamId, getExamByIdDto.InstructorId);
            if (!authResult.IsSuccess)
                return Result<ExamToReturnDto>.Failure(authResult.Error, "You are not authorized to view this exam");

            var exam = await _examRepository.GetExamByIdAsync(getExamByIdDto.ExamId);
            if (exam == null)
                return Result<ExamToReturnDto>.Failure(ErrorCode.NotFound, $"No exam found with ID {getExamByIdDto.ExamId}");

            return Result<ExamToReturnDto>.Success(exam);
        }

        public async Task<Result> EnrollStudentToExamAsync(AssignStudentToExamDto assignedStudentDto)
        {
            var validationResult = await _assignStudentToExamDtoValidator.ValidateAsync(assignedStudentDto);
            if (!validationResult.IsValid)
                return Result.ValidaitonFailure(validationResult);

            if (!await _examRepository.IsExamExistsAsync(assignedStudentDto.ExamId))
                return Result.Failure(ErrorCode.NotFound, $"Exam with ID {assignedStudentDto.ExamId} not found");

            if (!await _examRepository.IsStudentExistsAsync(assignedStudentDto.StudentId))
                return Result.Failure(ErrorCode.NotFound, $"Student with ID {assignedStudentDto.StudentId} not found");

            var authResult = await _authValidator.ValidateInstructorOfExamAsync(assignedStudentDto.ExamId, assignedStudentDto.InstructorId);
            if (!authResult.IsSuccess)
                return Result.Failure(authResult.Error, "You are not authorized to assign students to this exam");

            var notEnrolledResult = await _authValidator.ValidateStudentNotEnrolledInExamAsync(assignedStudentDto.ExamId, assignedStudentDto.StudentId);
            if (!notEnrolledResult.IsSuccess)
                return notEnrolledResult;

            var examEntity = await _unitOfWork.Repository<Exam>().GetByIdAsync(assignedStudentDto.ExamId);
            var courseEnrollmentResult = await _authValidator.ValidateStudentEnrolledInCourseAsync(examEntity!.CourseId, assignedStudentDto.StudentId);
            if (!courseEnrollmentResult.IsSuccess)
                return courseEnrollmentResult;

            var examAssignment = new ExamAssignment
            {
                ExamId = assignedStudentDto.ExamId,
                StudentId = assignedStudentDto.StudentId,
                AssignedDate = assignedStudentDto.AssignedDate
               
            };
            await _unitOfWork.Repository<ExamAssignment>().Add(examAssignment);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
                return Result.Failure(ErrorCode.DatabaseError, "Failed to enroll student to exam. Database error occurred.");

            return Result.Success();
        }

        public async Task<Result> ActivateExamAsync(ActivateExamDto activateExamDto)
        {
            var validationResult = await _activateExamDtoValidator.ValidateAsync(activateExamDto);
            if (!validationResult.IsValid)
                return Result.ValidaitonFailure(validationResult);

            var examResult = await _authValidator.ValidateAndGetExamAsync(activateExamDto.ExamId, activateExamDto.InstructorId);
            if (!examResult.IsSuccess)
                return Result.Failure(examResult.Error, examResult.ErrorMessage);

            var exam = examResult.Data!;
            if (exam.IsActive)
                return Result.Failure(ErrorCode.Conflict, $"Exam {activateExamDto.ExamId} is already active");

            await _examRepository.ActivateExamAsync(activateExamDto.ExamId);
            return Result.Success();
        }

        public async Task<Result> AddQuestionsToExamAsync(AddQuestionsToExamDto addQuestionsDto)
        {
            var validationResult = await _addQuestionsToExamDtoValidator.ValidateAsync(addQuestionsDto);
            if (!validationResult.IsValid)
                return Result.ValidaitonFailure(validationResult);

            var examResult = await _authValidator.ValidateAndGetExamAsync(addQuestionsDto.ExamId, addQuestionsDto.InstructorId);
            if (!examResult.IsSuccess)
                return Result.Failure(examResult.Error, examResult.ErrorMessage);

            var exam = examResult.Data!;
            var activeValidation = _authValidator.ValidateExamNotActive(exam);
            if (!activeValidation.IsSuccess)
                return activeValidation;

            var addResult = await _questionManager.AddQuestionsToExamAsync(addQuestionsDto.ExamId, addQuestionsDto.QuestionIds, exam.CourseId, exam.QuestionsCount);
            if (!addResult.IsSuccess)
                return addResult;

            var totalMark = await _questionManager.CalculateTotalMarkAsync(addQuestionsDto.ExamId);
            await _examRepository.UpdateExamFullMarkAsync(addQuestionsDto.ExamId, totalMark);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
                return Result.Failure(ErrorCode.DatabaseError, "Failed to add questions to exam. Database error occurred.");

            return Result.Success();
        }

        public async Task<Result> ReplaceExamQuestionsAsync(ReplaceExamQuestionsDto replaceQuestionsDto)
        {
            var validationResult = await _replaceExamQuestionsDtoValidator.ValidateAsync(replaceQuestionsDto);
            if (!validationResult.IsValid)
                return Result.ValidaitonFailure(validationResult);

            var examResult = await _authValidator.ValidateAndGetExamAsync(replaceQuestionsDto.ExamId, replaceQuestionsDto.InstructorId);
            if (!examResult.IsSuccess)
                return Result.Failure(examResult.Error, examResult.ErrorMessage);

            var exam = examResult.Data!;
            var activeValidation = _authValidator.ValidateExamNotActive(exam);
            if (!activeValidation.IsSuccess)
                return activeValidation;

            var replaceResult = await _questionManager.ReplaceQuestionsAsync(replaceQuestionsDto.ExamId, replaceQuestionsDto.QuestionIds, exam.CourseId, exam.QuestionsCount);
            if (!replaceResult.IsSuccess)
                return replaceResult;

            var totalMark = await _questionManager.CalculateTotalMarkAsync(replaceQuestionsDto.ExamId);
            await _examRepository.UpdateExamFullMarkAsync(replaceQuestionsDto.ExamId, totalMark);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
                return Result.Failure(ErrorCode.DatabaseError, "Failed to replace exam questions. Database error occurred.");

            return Result.Success();
        }

        public async Task<Result> RemoveQuestionFromExamAsync(RemoveQuestionFromExamDto removeQuestionDto)
        {
            var validationResult = await _removeQuestionFromExamDtoValidator.ValidateAsync(removeQuestionDto);
            if (!validationResult.IsValid)
                return Result.ValidaitonFailure(validationResult);

            var examResult = await _authValidator.ValidateAndGetExamAsync(removeQuestionDto.ExamId, removeQuestionDto.InstructorId);
            if (!examResult.IsSuccess)
                return Result.Failure(examResult.Error, examResult.ErrorMessage);

            var exam = examResult.Data!;
            var activeValidation = _authValidator.ValidateExamNotActive(exam);
            if (!activeValidation.IsSuccess)
                return activeValidation;

            var removeResult = await _questionManager.RemoveQuestionAsync(removeQuestionDto.ExamId, removeQuestionDto.QuestionId);
            if (!removeResult.IsSuccess)
                return removeResult;

            var totalMark = await _questionManager.CalculateTotalMarkAsync(removeQuestionDto.ExamId);
            await _examRepository.UpdateExamFullMarkAsync(removeQuestionDto.ExamId, totalMark);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
                return Result.Failure(ErrorCode.DatabaseError, "Failed to remove question from exam. Database error occurred.");

            return Result.Success();
        }

        public async Task<bool> IsInstructorOfExamAsync(Guid examId, Guid instructorId)
        {
            return await _unitOfWork.Repository<Exam>()
                .IsExistsByCriteriaAsync(e => e.Id == examId && e.InstructorId == instructorId);
        }
    }
}
