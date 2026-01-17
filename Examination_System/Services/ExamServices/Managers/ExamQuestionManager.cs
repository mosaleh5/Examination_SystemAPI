using Examination_System.Common;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.ExamServices.Managers
{
    public class ExamQuestionManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamQuestionManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<Question>>> SelectBalancedQuestionsAsync(Guid courseId, int totalCount)
        {
            var (simpleCount, mediumCount, hardCount) = GetBalancedCounts(totalCount);

            var availableQuestions = await _unitOfWork.Repository<Question>()
                .GetByCriteria(q => q.CourseId == courseId)
                .ToListAsync();

            if (!HasSufficientQuestions(availableQuestions, simpleCount, mediumCount, hardCount))
            {
                return Result<List<Question>>.Failure(
                    ErrorCode.BadRequest,
                    "Not enough questions in the course to create a balanced exam");
            }

            var selectedQuestions = SelectQuestionsByLevel(availableQuestions, simpleCount, mediumCount, hardCount);
            if (selectedQuestions.Count != totalCount)
            {
                return Result<List<Question>>.Failure(
                    ErrorCode.BadRequest,
                    "Could not select the required number of questions");
            }

            return Result<List<Question>>.Success(selectedQuestions);
        }

        public async Task<Result> AddQuestionsToExamAsync(Guid examId, List<Guid> questionIds, Guid courseId, int maxQuestionsCount)
        {
            var existingQuestionIds = await GetExistingQuestionIdsAsync(examId);
            var newQuestionIds = questionIds.Except(existingQuestionIds).ToList();

            if (!newQuestionIds.Any())
            {
                return Result.Failure(
                    ErrorCode.Conflict,
                    "All provided questions are already added to this exam");
            }

            var questions = await _unitOfWork.Repository<Question>()
                .GetAll()
                .Where(q => newQuestionIds.Contains(q.Id))
                .ToListAsync();

            if (questions.Count != newQuestionIds.Count)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    "One or more questions were not found");
            }

            if (existingQuestionIds.Count + newQuestionIds.Count > maxQuestionsCount)
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    $"Cannot add more than {maxQuestionsCount} questions to this exam");
            }

            if (questions.Any(q => q.CourseId != courseId))
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    "All questions must belong to the same course as the exam");
            }

            var examQuestions = newQuestionIds.Select(qId => new ExamQuestion
            {
                ExamId = examId,
                QuestionId = qId
            }).ToList();

            await _unitOfWork.Repository<ExamQuestion>().AddCollectionAsync(examQuestions);

            return Result.Success();
        }

        public async Task<Result> ReplaceQuestionsAsync(Guid examId, List<Guid> questionIds, Guid courseId, int maxQuestionsCount)
        {
            if (questionIds.Count > maxQuestionsCount)
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    $"Cannot add more than {maxQuestionsCount} questions to this exam");
            }

            var questions = await _unitOfWork.Repository<Question>()
                .GetAll()
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            if (questions.Count != questionIds.Distinct().Count())
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    "One or more questions were not found");
            }

            if (questions.Any(q => q.CourseId != courseId))
            {
                return Result.Failure(
                    ErrorCode.BadRequest,
                    "All questions must belong to the same course as the exam");
            }

            await RemoveAllExamQuestionsAsync(examId);

            var newExamQuestions = questionIds.Select(qId => new ExamQuestion
            {
                ExamId = examId,
                QuestionId = qId
            }).ToList();

            foreach (var eq in newExamQuestions)
            {
                await _unitOfWork.Repository<ExamQuestion>().AddAsync(eq);
            }

            return Result.Success();
        }

        public async Task<Result> RemoveQuestionAsync(Guid examId, Guid questionId)
        {
            var examQuestion = await _unitOfWork.Repository<ExamQuestion>()
                .GetAll()
                .FirstOrDefaultAsync(eq => eq.ExamId == examId && eq.QuestionId == questionId);

            if (examQuestion == null)
            {
                return Result.Failure(
                    ErrorCode.NotFound,
                    $"Question {questionId} is not part of exam {examId}");
            }

           var result =  await _unitOfWork.Repository<ExamQuestion>().DeleteAsync(examQuestion.Id);
            
            return result ? Result.Success() : Result.Failure(ErrorCode.FieldToRemoveQuestion, "Field To Remove");
        }

        public async Task<int> CalculateTotalMarkAsync(Guid examId)
        {
            var questionIds = await GetExistingQuestionIdsAsync(examId);

            if (!questionIds.Any())
                return 0;

            return (int)await _unitOfWork.Repository<Question>()
                .GetAll()
                .Where(q => questionIds.Contains(q.Id))
                .SumAsync(q => q.Mark);
        }

        private async Task<List<Guid>> GetExistingQuestionIdsAsync(Guid examId)
        {
            return await _unitOfWork.Repository<ExamQuestion>()
                .GetAll()
                .Where(eq => eq.ExamId == examId)
                .Select(eq => eq.QuestionId)
                .ToListAsync();
        }

        private async Task RemoveAllExamQuestionsAsync(Guid examId)
        {
            var existingExamQuestions = await _unitOfWork.Repository<ExamQuestion>()
                .GetAll()
                .Where(eq => eq.ExamId == examId)
                .ToListAsync();

            foreach (var eq in existingExamQuestions)
            {
                await _unitOfWork.Repository<ExamQuestion>().DeleteAsync(eq.Id);
            }
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

        private static List<Question> SelectQuestionsByLevel(IEnumerable<Question> questions, int simpleNeeded, int mediumNeeded, int hardNeeded)
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
    }
}