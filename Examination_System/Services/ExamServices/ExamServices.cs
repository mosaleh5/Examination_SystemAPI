using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.Common;
using Examination_System.DTOs.Exam;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Examination_System.ViewModels.Exam;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Examination_System.Models.Question;

namespace Examination_System.Services.ExamServices
{
    public class ExamServices : GenericServices<Exam , int>  , IExamServices
    {
    

        public ExamServices(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) 
        {
         
        }

        public Task CreateExamAsync(CreateExamDto createExamDto)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<ExamToReturnDto>> CreateAutomaticExam(CreateAutomaticExamDto createExamDto)
        {
            if (createExamDto is null)
            {
                return new BadRequestObjectResult("Invalid exam data.");
            }

            if (createExamDto.QuestionsCount <= 0)
            {
                return new BadRequestObjectResult("QuestionsCount must be greater than zero.");
            }

            // Prepare exam entity once
            var exam = _mapper.Map<Exam>(createExamDto);
            await _unitOfWork.Repository<Exam, int>().AddAsync(exam);
            await _unitOfWork.CompleteAsync();

            var (simpleCount, mediumCount, hardCount) = GetBalancedCounts(createExamDto.QuestionsCount);

            var availableQuestions = _unitOfWork.Repository<Question, int>()
                .GetByCriteria(q => q.CourseId == createExamDto.CourseId)
                .ToList();

            if (!HasSufficientQuestions(availableQuestions, simpleCount, mediumCount, hardCount))
            {
                return new BadRequestObjectResult("Not enough questions in the course to create a balanced exam.");
            }

            var selectedQuestions = SelectBalancedQuestions(availableQuestions, simpleCount, mediumCount, hardCount);
            if (selectedQuestions.Count != createExamDto.QuestionsCount)
            {
                return new BadRequestObjectResult("Could not select the required number of questions.");
            }

            var totalMark = (int)selectedQuestions.Sum(q => q.mark);

            // Update exam full mark
            await _unitOfWork.Repository<Exam, int>().ExecuteUpdateAsync(exam.Id, e => e.Fullmark, totalMark);

            // Persist exam questions
            var examQuestions = selectedQuestions.Select(q => new ExamQuestion
            {
                QuestionId = q.Id,
                ExamId = exam.Id
            }).ToList();

            foreach (var eq in examQuestions)
            {
                await _unitOfWork.Repository<ExamQuestion, int>().AddAsync(eq);
            }

            await _unitOfWork.CompleteAsync();

            var examToReturn = _mapper.Map<ExamToReturnDto>(exam);
            return examToReturn;
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

            if (simpleNeeded > simpleAvailable) return false;
            if (mediumNeeded > mediumAvailable) return false;
            if (hardNeeded > hardAvailable) return false;

            return true;
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

        public async Task<Result<ExamToReturnDto>> CreateExam(CreateExamDto createExamDto)
        {
        
            if (!await IscourseExists(createExamDto.CourseId))
            {
                return Result<ExamToReturnDto>.Failure(
                    ErrorCode.CourseIsNotFound,
                    "Course with Id {createExamDto.CourseId} does not exist"
                    );  
            }

           
            if (! await IsInstructorOfCourse(createExamDto.CourseId , createExamDto.InstructorId))
            {
                return Result<ExamToReturnDto>.Failure(
                     ErrorCode.CourseIsNotFound,
                     "Course with Id {createExamDto.CourseId} does not exist"
                     );
            }

            var exam = _mapper.Map<Exam>(createExamDto);
            await _unitOfWork.Repository<Exam, int>().AddAsync(exam);
            await _unitOfWork.CompleteAsync();

            var examToReturn = _mapper.Map<ExamToReturnDto>(exam);
            return Result<ExamToReturnDto>.Success(examToReturn);
        }

        private async Task<bool> IscourseExists(int courseId)
        {
            return await _unitOfWork.Repository<Course, int>()
                .IsExistsAsync(courseId);
        }

        private async Task<bool> IsInstructorOfCourse(int courseId, string instructorId)
        {
            return await _unitOfWork.Repository<Course, int>()
                .IsExistsByCriteriaAsync(c => c.Id == courseId && c.InstructorId == instructorId);
                   
        }
        public async Task<IEnumerable<ExamToReturnDto>> GetAllExamsForInstructor(string? instructorId)
        {
            if (instructorId.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(instructorId));
            }

            var examSpecifications = new ExamSpecifications(e => e.InstructorId == instructorId);
            var examsQuery = _unitOfWork.Repository<Exam, int>().GetAllWithSpecificationAsync(examSpecifications);
            var exams = await examsQuery.ProjectTo<ExamToReturnDto>(_mapper.ConfigurationProvider).ToListAsync();

            if (exams == null || exams.Count == 0)
            {
                throw new InvalidOperationException("No exams found for the given instructor.");
            }

            return exams;
        }
    
        public async Task<bool> EnrollStudentToExamAsync(int examId, string studentId)
        {
            if (!await _unitOfWork.Repository<Exam, int>().IsExistsAsync(examId))
            {
                throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            }

            if (!await _unitOfWork.Repository<Student, string>().IsExistsAsync(studentId))
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found.");
            }

            var examSpecifications = new ExamSpecifications(e => e.Id == examId);
            var exam = await _unitOfWork.Repository<Exam, int>().GetByIdWithSpecification(examSpecifications);
            if (exam == null)
            {
                throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            }

            if (!await IsStudentAlreadyEnrolledToCourseAsync(exam.CourseId, studentId))
            {
                throw new InvalidOperationException($"Student {studentId} is not assigned to the course {exam.CourseId}. You must assign the student to the course first.");
            }

            if (await IsStudentAlreadyEnrolledToThisExam(examId, studentId))
            {
                throw new InvalidOperationException($"Student {studentId} is already assigned to exam {examId}.");
            }

            var examAssignment = new ExamAssignment
            {
                ExamId = examId,
                StudentId = studentId,
                AssignedDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<ExamAssignment, int>().Add(examAssignment);
            var result = await _unitOfWork.SaveChangesAsync() > 0;
            return result;
        }

        private async Task<bool> IsStudentAlreadyEnrolledToThisExam(int examId, string studentId)
        {
            return await _unitOfWork.Repository<ExamAssignment, int>().GetAll()
                .AnyAsync(e => e.ExamId == examId && e.StudentId == studentId);
        }

        private async Task<bool> IsStudentAlreadyEnrolledToCourseAsync(int courseId, string studentId)
        {
            return await _unitOfWork.Repository<CourseEnrollment, int>().GetAll()
                .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
        }

      

        public async Task<bool> IsInstructorOfExamAsync(int examId, string instructorId)
        {
            var exam = await _unitOfWork.Repository<Exam, int>().GetByIdAsync(examId);
            
            if (exam == null)
            {
                throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            }

            return exam.InstructorId == instructorId;
        }

        public async Task<bool> ActivateExamAsync(int examId, string instructorId)
        {
            var exam = await _unitOfWork.Repository<Exam, int>().GetByIdAsync(examId);
            if (exam == null)
            {
                throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            }

            if (exam.InstructorId != instructorId)
            {
                throw new UnauthorizedAccessException($"Instructor {instructorId} is not authorized to activate exam {examId}.");
            }

            if (exam.IsActive)
            {
                throw new InvalidOperationException($"Exam {examId} is already active.");
            }

            await _unitOfWork.Repository<Exam, int>().ExecuteUpdateAsync(examId, e => e.IsActive, true);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task AddQuestionsToExamAsync(int examId, List<int> questionIds)
        {
            if (questionIds == null || questionIds.Count == 0)
            {
                throw new ArgumentException("Question IDs cannot be null or empty.");
            }

            var exam = await _unitOfWork.Repository<Exam, int>().GetByIdAsync(examId);
            if (exam == null)
            {
                throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            }

            if (exam.IsActive)
            {
                throw new InvalidOperationException($"Cannot modify questions of an active exam {examId}.");
            }

            // Get existing question IDs for this exam
            var existingQuestionIds = await _unitOfWork.Repository<ExamQuestion, int>().GetAll()
                .Where(eq => eq.ExamId == examId)
                .Select(eq => eq.QuestionId)
                .ToListAsync();

            // Filter out duplicates
            var newQuestionIds = questionIds.Except(existingQuestionIds).ToList();

            if (newQuestionIds.Count == 0)
            {
                throw new InvalidOperationException("All provided questions are already added to this exam.");
            }

            // Validate all questions exist and belong to the same course
            var questions = await _unitOfWork.Repository<Question, int>().GetAll()
                .Where(q => newQuestionIds.Contains(q.Id))
                .ToListAsync();

            if (questions.Count != newQuestionIds.Count)
            {
                throw new KeyNotFoundException("One or more questions were not found ");
            }
            if(exam.QuestionsCount < newQuestionIds.Count + existingQuestionIds.Count)
            {
                throw new InvalidOperationException($"you can not add more than {exam.QuestionsCount} Questions for this exam");
            }
            if (questions.Any(q => q.CourseId != exam.CourseId))
            {
                throw new InvalidOperationException("All questions must belong to the same course as the exam.");
            }

            // Add new questions
            var examQuestions = newQuestionIds.Select(qId => new ExamQuestion
            {
                ExamId = examId,
                QuestionId = qId
            }).ToList();

           
        await _unitOfWork.Repository<ExamQuestion, int>().AddCollectionAsync(examQuestions);
            

            // Update exam full mark
            var totalMark = existingQuestionIds.Count + newQuestionIds.Count > 0
                ? (int)await _unitOfWork.Repository<Question, int>().GetAll()
                    .Where(q => existingQuestionIds.Concat(newQuestionIds).Contains(q.Id))
                    .SumAsync(q => q.mark)
                : 0;

            await _unitOfWork.Repository<Exam, int>().ExecuteUpdateAsync(examId, e => e.Fullmark, totalMark);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ReplaceExamQuestionsAsync(int examId, List<int> questionIds)
        {
            var exam = await _unitOfWork.Repository<Exam, int>().GetByIdAsync(examId);

            if (exam.QuestionsCount < questionIds.Count)
            {
                throw new InvalidOperationException($"you can not add more than {exam.QuestionsCount} Questions for this exam");
            }
            if (questionIds == null || questionIds.Count == 0)
            {
                throw new ArgumentException("Question IDs cannot be null or empty.");
            }

            if (exam == null)
            {
                throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            }

            if (exam.IsActive)
            {
                throw new InvalidOperationException($"Cannot modify questions of an active exam {examId}.");
            }

            // Validate all questions exist and belong to the same course
            var questions = await _unitOfWork.Repository<Question, int>().GetAll()
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            if (questions.Count != questionIds.Distinct().Count())
            {
                throw new KeyNotFoundException("One or more questions were not found.");
            }

            if (questions.Any(q => q.CourseId != exam.CourseId))
            {
                throw new InvalidOperationException("All questions must belong to the same course as the exam.");
            }

            // Remove all existing questions
            var existingExamQuestions = await _unitOfWork.Repository<ExamQuestion, int>().GetAll()
                .Where(eq => eq.ExamId == examId)
                .ToListAsync();

            foreach (var eq in existingExamQuestions)
            {
                await _unitOfWork.Repository<ExamQuestion, int>().DeleteAsync(eq.Id);
            }

            // Add new questions
            var newExamQuestions = questionIds.Select(qId => new ExamQuestion
            {
                ExamId = examId,
                QuestionId = qId
            }).ToList();

            foreach (var eq in newExamQuestions)
            {
                await _unitOfWork.Repository<ExamQuestion, int>().AddAsync(eq);
            }

            // Update exam full mark
            var totalMark = (int)questions.Sum(q => q.mark);
            await _unitOfWork.Repository<Exam, int>().ExecuteUpdateAsync(examId, e => e.Fullmark, totalMark);

            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveQuestionFromExamAsync(int examId, int questionId)
        {
            var exam = await _unitOfWork.Repository<Exam, int>().GetByIdAsync(examId);
            if (exam == null)
            {
                throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            }

            if (exam.IsActive)
            {
                throw new InvalidOperationException($"Cannot modify questions of an active exam {examId}.");
            }

            var examQuestion = await _unitOfWork.Repository<ExamQuestion, int>().GetAll()
                .FirstOrDefaultAsync(eq => eq.ExamId == examId && eq.QuestionId == questionId);

            if (examQuestion == null)
            {
                throw new KeyNotFoundException($"Question {questionId} is not part of exam {examId}.");
            }

            await _unitOfWork.Repository<ExamQuestion, int>().DeleteAsync(examQuestion.Id);

            // Recalculate full mark
            var remainingQuestionIds = await _unitOfWork.Repository<ExamQuestion, int>().GetAll()
                .Where(eq => eq.ExamId == examId)
                .Select(eq => eq.QuestionId)
                .ToListAsync();

            var totalMark = remainingQuestionIds.Count > 0
                ? (int)await _unitOfWork.Repository<Question, int>().GetAll()
                    .Where(q => remainingQuestionIds.Contains(q.Id))
                    .SumAsync(q => q.mark)
                : 0;

            await _unitOfWork.Repository<Exam, int>().ExecuteUpdateAsync(examId, e => e.Fullmark, totalMark);
            await _unitOfWork.CompleteAsync();
        }
    }
}
