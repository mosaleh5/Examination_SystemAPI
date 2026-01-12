using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.Common;
using Examination_System.DTOs.Exam;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
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
            IValidator<GetExamByIdDto> getExamByIdDtoValidator)
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
        }

        public async Task<Result<ExamToReturnDto>> CreateExam(CreateExamDto createExamDto)
        {
            var validationResult = await _createExamValidator.ValidateAsync(createExamDto);
            if (!validationResult.IsValid)
            {
                return Result<ExamToReturnDto>.ValidaitonFailure(validationResult);
            }

            if (!await IsCourseExists(createExamDto.CourseId))
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {createExamDto.CourseId} not found");
            }

            if (!await IsInstructorOfCourse(createExamDto.CourseId, createExamDto.InstructorId))
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.Forbidden,
                    "You are not the instructor of this course");
            }

            var exam = _mapper.Map<Exam>(createExamDto);
            await _unitOfWork.Repository<Exam>().AddAsync(exam);
            
            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to create exam. Database error occurred.");
            }

            var examToReturn = _mapper.Map<ExamToReturnDto>(exam);
            return Result<ExamToReturnDto>.Success(examToReturn);
        }

        public async Task<Result<ExamToReturnDto>> CreateAutomaticExam(CreateAutomaticExamDto createAutomaticExamDto)
        {
            var validationResult = await _createAutomaticExamValidator.ValidateAsync(createAutomaticExamDto);
            if (!validationResult.IsValid)
            {
                return Result<ExamToReturnDto>.ValidaitonFailure(validationResult);
            }

            if (!await IsCourseExists(createAutomaticExamDto.CourseId))
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.NotFound,
                    $"Course with ID {createAutomaticExamDto.CourseId} not found");
            }

            if (!await IsInstructorOfCourse(createAutomaticExamDto.CourseId, createAutomaticExamDto.InstructorId))
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.Forbidden,
                    "You are not the instructor of this course");
            }

            var exam = _mapper.Map<Exam>(createAutomaticExamDto);
            await _unitOfWork.Repository<Exam>().AddAsync(exam);
            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to create automatic exam. Database error occurred.");
            }

            var (simpleCount, mediumCount, hardCount) = GetBalancedCounts(createAutomaticExamDto.QuestionsCount);

            var availableQuestions = _unitOfWork.Repository<Question>()
                .GetByCriteria(q => q.CourseId == createAutomaticExamDto.CourseId)
                .ToList();

            if (!HasSufficientQuestions(availableQuestions, simpleCount, mediumCount, hardCount))
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.BadRequest,
                    "Not enough questions in the course to create a balanced exam");
            }

            var selectedQuestions = SelectBalancedQuestions(availableQuestions, simpleCount, mediumCount, hardCount);
            if (selectedQuestions.Count != createAutomaticExamDto.QuestionsCount)
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.BadRequest,
                    "Could not select the required number of questions");
            }

            var totalMark = (int)selectedQuestions.Sum(q => q.mark);
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

            var ExamrowsAffected = await _unitOfWork.CompleteAsync();
            if (ExamrowsAffected < 1)
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to create automatic exam. Database error occurred.");
            }
            
            var GetExamDto = new GetExamByIdDto
            {
                ExamId = exam.Id,
                InstructorId = createAutomaticExamDto.InstructorId
            };
            var result = await GetExamsForInstructorById(GetExamDto);
            if(!result.IsSuccess)
            {
                var examToReturn = _mapper.Map<ExamToReturnDto>(exam);
                return Result<ExamToReturnDto>.Success(examToReturn , "Exam created succfully if you want it in details you can get it from get by id");
            }

            return result;
        }

        public async Task<Result<IEnumerable<ExamToReturnDto>>> GetAllExamsForInstructor(Guid? instructorId)
        {
            if (instructorId == null)
            {
                return Result<IEnumerable<ExamToReturnDto>>.Failure(
                    ErrorCode.ValidationError,
                    "Instructor ID is required");
            }

            var examSpecifications = new ExamSpecifications(e => e.InstructorId == instructorId);
            var examsQuery = _unitOfWork.Repository<Exam>().GetAllWithSpecificationAsync(examSpecifications);
            var exams = await examsQuery
                .ProjectTo<ExamToReturnDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!exams.Any())
            {
                return Result<IEnumerable<ExamToReturnDto>>.Failure(
                    ErrorCode.NotFound,
                    $"No exams found for instructor {instructorId}");
            }

            return Result<IEnumerable<ExamToReturnDto>>.Success(exams);
        }

        public async Task<Result> EnrollStudentToExamAsync(AssignStudentToExamDto assignedStudentDto)
        {
            var validationResult = await _assignStudentToExamDtoValidator.ValidateAsync(assignedStudentDto);
            if (!validationResult.IsValid)
            {
                return Result.ValidaitonFailure(validationResult);
            }

            if (!await _unitOfWork.Repository<Exam>().IsExistsAsync(assignedStudentDto.ExamId))
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Exam with ID {assignedStudentDto.ExamId} not found");
            }

            if (!await _unitOfWork.Repository<Student>().IsExistsAsync(assignedStudentDto.StudentId))
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Student with ID {assignedStudentDto.StudentId} not found");
            }

            if (!await IsInstructorOfExamAsync(assignedStudentDto.ExamId, assignedStudentDto.InstructorId))
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to assign students to this exam");
            }

            if (await IsStudentAlreadyEnrolledToThisExam(assignedStudentDto.ExamId, assignedStudentDto.StudentId))
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Student {assignedStudentDto.StudentId} is already assigned to exam {assignedStudentDto.ExamId}");
            }

            var examEntity = await _unitOfWork.Repository<Exam>().GetByIdAsync(assignedStudentDto.ExamId);
            var courseId = examEntity.CourseId ;



            if (!await IsStudentAlreadyEnrolledToCourseAsync(courseId, assignedStudentDto.StudentId))
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    $"Student {assignedStudentDto.StudentId} is not enrolled in course {courseId}. Student must be enrolled in the course first");
            }



            var examAssignment = _mapper.Map<ExamAssignment>(assignedStudentDto);

            await _unitOfWork.Repository<ExamAssignment>().Add(examAssignment);
            
            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to enroll student to exam. Database error occurred.");
            }

            return Result.Success();
        }

        public async Task<Result> ActivateExamAsync(ActivateExamDto activateExamDto)
        {
            var validationResult = await _activateExamDtoValidator.ValidateAsync(activateExamDto);
            if (!validationResult.IsValid)
            {
                return Result.ValidaitonFailure(validationResult);
            }

            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(activateExamDto.ExamId);
            if (exam == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Exam with ID {activateExamDto.ExamId} not found");
            }

            if (exam.InstructorId != activateExamDto.InstructorId)
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to activate this exam");
            }

            if (exam.IsActive)
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Exam {activateExamDto.ExamId} is already active");
            }

            //await _unitOfWork.Repository<Exam>().ExecuteUpdateAsync(activateExamDto.ExamId, e => e.IsActive, true);
            var affectedRows = await _unitOfWork.Repository<Exam>().ExecuteUpdateAsync<Exam>(
                course => course.Id == activateExamDto.ExamId,
                setters => setters
                    .SetProperty(c => c.IsActive, true)
                    .SetProperty(c => c.UpdatedAt, DateTime.UtcNow)
            );

            if (affectedRows < 1)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to activate exam. Database error occurred.");
            }

            return Result.Success();
        }

        public async Task<Result> AddQuestionsToExamAsync(AddQuestionsToExamDto addQuestionsDto)
        {
            var validationResult = await _addQuestionsToExamDtoValidator.ValidateAsync(addQuestionsDto);
            if (!validationResult.IsValid)
            {
                return Result.ValidaitonFailure(validationResult);
            }

            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(addQuestionsDto.ExamId);
            if (exam == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Exam with ID {addQuestionsDto.ExamId} not found");
            }

            if (exam.InstructorId != addQuestionsDto.InstructorId)
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to modify this exam");
            }

            if (exam.IsActive)
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Cannot modify questions of an active exam {addQuestionsDto.ExamId}");
            }

            var existingQuestionIds = await _unitOfWork.Repository<ExamQuestion>().GetAll()
                .Where(eq => eq.ExamId == addQuestionsDto.ExamId)
                .Select(eq => eq.QuestionId)
                .ToListAsync();

            var newQuestionIds = addQuestionsDto.QuestionIds.Except(existingQuestionIds).ToList();

            if (!newQuestionIds.Any())
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    "All provided questions are already added to this exam");
            }

            var questions = await _unitOfWork.Repository<Question>().GetAll()
                .Where(q => newQuestionIds.Contains(q.Id))
                .ToListAsync();

            if (questions.Count != newQuestionIds.Count)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    "One or more questions were not found");
            }

            if (exam.QuestionsCount < newQuestionIds.Count + existingQuestionIds.Count)
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    $"Cannot add more than {exam.QuestionsCount} questions to this exam");
            }

            if (questions.Any(q => q.CourseId != exam.CourseId))
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    "All questions must belong to the same course as the exam");
            }

            var examQuestions = newQuestionIds.Select(qId => new ExamQuestion
            {
                ExamId = addQuestionsDto.ExamId,
                QuestionId = qId
            }).ToList();

            await _unitOfWork.Repository<ExamQuestion>().AddCollectionAsync(examQuestions);

            var totalMark = existingQuestionIds.Count + newQuestionIds.Count > 0
                ? (int)await _unitOfWork.Repository<Question>().GetAll()
                    .Where(q => existingQuestionIds.Concat(newQuestionIds).Contains(q.Id))
                    .SumAsync(q => q.mark)
                : 0;

            await _unitOfWork.Repository<Exam>().ExecuteUpdateAsync(addQuestionsDto.ExamId, e => e.Fullmark, totalMark);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to add questions to exam. Database error occurred.");
            }

            return Result.Success();
        }

        public async Task<Result> ReplaceExamQuestionsAsync(ReplaceExamQuestionsDto replaceQuestionsDto)
        {
            var validationResult = await _replaceExamQuestionsDtoValidator.ValidateAsync(replaceQuestionsDto);
            if (!validationResult.IsValid)
            {
                return Result.ValidaitonFailure(validationResult);
            }

            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(replaceQuestionsDto.ExamId);
            if (exam == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Exam with ID {replaceQuestionsDto.ExamId} not found");
            }

            if (exam.InstructorId != replaceQuestionsDto.InstructorId)
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to modify this exam");
            }

            if (exam.IsActive)
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Cannot modify questions of an active exam {replaceQuestionsDto.ExamId}");
            }

            if (exam.QuestionsCount < replaceQuestionsDto.QuestionIds.Count)
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    $"Cannot add more than {exam.QuestionsCount} questions to this exam");
            }

            var questions = await _unitOfWork.Repository<Question>().GetAll()
                .Where(q => replaceQuestionsDto.QuestionIds.Contains(q.Id))
                .ToListAsync();

            if (questions.Count != replaceQuestionsDto.QuestionIds.Distinct().Count())
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    "One or more questions were not found");
            }

            if (questions.Any(q => q.CourseId != exam.CourseId))
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    "All questions must belong to the same course as the exam");
            }

            var existingExamQuestions = await _unitOfWork.Repository<ExamQuestion>().GetAll()
                .Where(eq => eq.ExamId == replaceQuestionsDto.ExamId)
                .ToListAsync();

            foreach (var eq in existingExamQuestions)
            {
                await _unitOfWork.Repository<ExamQuestion>().DeleteAsync(eq.Id);
            }

            var newExamQuestions = replaceQuestionsDto.QuestionIds.Select(qId => new ExamQuestion
            {
                ExamId = replaceQuestionsDto.ExamId,
                QuestionId = qId
            }).ToList();

            foreach (var eq in newExamQuestions)
            {
                await _unitOfWork.Repository<ExamQuestion>().AddAsync(eq);
            }

            var totalMark = (int)questions.Sum(q => q.mark);
            await _unitOfWork.Repository<Exam>().ExecuteUpdateAsync(replaceQuestionsDto.ExamId, e => e.Fullmark, totalMark);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to replace exam questions. Database error occurred.");
            }

            return Result.Success();
        }

        public async Task<Result> RemoveQuestionFromExamAsync(RemoveQuestionFromExamDto removeQuestionDto)
        {
            var validationResult = await _removeQuestionFromExamDtoValidator.ValidateAsync(removeQuestionDto);
            if (!validationResult.IsValid)
            {
                return Result.ValidaitonFailure(validationResult);
            }

            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(removeQuestionDto.ExamId);
            if (exam == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Exam with ID {removeQuestionDto.ExamId} not found");
            }

            if (exam.InstructorId != removeQuestionDto.InstructorId)
            {
                return Result.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to modify this exam");
            }

            if (exam.IsActive)
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    $"Cannot modify questions of an active exam {removeQuestionDto.ExamId}");
            }

            var examQuestion = await _unitOfWork.Repository<ExamQuestion>().GetAll()
                .FirstOrDefaultAsync(eq => eq.ExamId == removeQuestionDto.ExamId && eq.QuestionId == removeQuestionDto.QuestionId);

            if (examQuestion == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Question {removeQuestionDto.QuestionId} is not part of exam {removeQuestionDto.ExamId}");
            }

            await _unitOfWork.Repository<ExamQuestion>().DeleteAsync(examQuestion.Id);

            var remainingQuestionIds = await _unitOfWork.Repository<ExamQuestion>().GetAll()
                .Where(eq => eq.ExamId == removeQuestionDto.ExamId)
                .Select(eq => eq.QuestionId)
                .ToListAsync();

            var totalMark = remainingQuestionIds.Any()
                ? (int)await _unitOfWork.Repository<Question>().GetAll()
                    .Where(q => remainingQuestionIds.Contains(q.Id))
                    .SumAsync(q => q.mark)
                : 0;

            await _unitOfWork.Repository<Exam>().ExecuteUpdateAsync(removeQuestionDto.ExamId, e => e.Fullmark, totalMark);

            var rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected < 1)
            {
                return Result.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to remove question from exam. Database error occurred.");
            }

            return Result.Success();
        }

        // Private helper methods
        private async Task<bool> IsCourseExists(Guid courseId)
        {
            return await _unitOfWork.Repository<Course>().IsExistsAsync(courseId);
        }

        private async Task<bool> IsInstructorOfCourse(Guid courseId, Guid instructorId)
        {
            return await _unitOfWork.Repository<Course>()
                .IsExistsByCriteriaAsync(c => c.Id == courseId && c.InstructorId == instructorId);
        }

        private async Task<bool> IsStudentAlreadyEnrolledToThisExam(Guid examId, Guid studentId)
        {
            return await _unitOfWork.Repository<ExamAssignment>().GetAll()
                .AnyAsync(e => e.ExamId == examId && e.StudentId == studentId);
        }

        private async Task<bool> IsStudentAlreadyEnrolledToCourseAsync(Guid courseId, Guid studentId)
        {
            return await _unitOfWork.Repository<CourseEnrollment>().GetAll()
                .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        }

        private static (int simple, int medium, int hard) GetBalancedCounts(int totalCount)
        {
            var perLevel = totalCount / 3;
            var medium = totalCount - (2 * perLevel);
            return (perLevel, medium, perLevel);
        }

        private static bool HasSufficientQuestions(IEnumerable<Question> questions, int simpleNeeded, int mediumNeeded, int hardNeeded)
        {
            var simpleAvailable = questions.Count(q => q.Level == QuestionLevel.Simple);
            var mediumAvailable = questions.Count(q => q.Level == QuestionLevel.Medium);
            var hardAvailable = questions.Count(q => q.Level == QuestionLevel.Hard);

            return simpleNeeded <= simpleAvailable &&
                   mediumNeeded <= mediumAvailable &&
                   hardNeeded <= hardAvailable;
        }

        private static List<Question> SelectBalancedQuestions(IEnumerable<Question> questions, int simpleNeeded, int mediumNeeded, int hardNeeded)
        {
            var selected = new List<Question>();

            selected.AddRange(
                questions.Where(q => q.Level == QuestionLevel.Simple)
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(simpleNeeded));

            selected.AddRange(
                questions.Where(q => q.Level == QuestionLevel.Medium)
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(mediumNeeded));

            selected.AddRange(
                questions.Where(q => q.Level == QuestionLevel.Hard)
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(hardNeeded));

            return selected;
        }

        public async Task<bool> IsInstructorOfExamAsync(Guid examId, Guid instructorId)
        {
            return await _unitOfWork.Repository<Exam>()
                .IsExistsByCriteriaAsync(e => e.Id == examId && e.InstructorId == instructorId);
        }

        public async Task<Result<ExamToReturnDto>> GetExamsForInstructorById(GetExamByIdDto getExamByIdDto)
        {
            var validationResult = _getExamByIdDtoValidator.Validate(getExamByIdDto);
            if (!validationResult.IsValid)
            {
                return  Result<ExamToReturnDto>.ValidaitonFailure(validationResult);
            }
            if(!await IsInstructorOfExamAsync(getExamByIdDto.ExamId, getExamByIdDto.InstructorId))
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.Forbidden,
                    "You are not authorized to view this exam");
            }
            var examSpecifications = new ExamSpecifications(e => e.Id == getExamByIdDto.ExamId);
            var exam = await _unitOfWork.Repository<Exam>()
                .GetAllWithSpecificationAsync(examSpecifications)
                .ProjectTo<ExamToReturnDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if (exam == null)
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.NotFound,
                    $"No exams found for instructor {getExamByIdDto.InstructorId} with exam id {getExamByIdDto.ExamId}");
            }
           
            return Result<ExamToReturnDto>.Success(exam);

        }
    }
}
