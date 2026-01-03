//using Examination_System.DTOs.Exam;
//using Examination_System.Models;

//namespace Examination_System.Mappers
//{
//    public static class ExamMapper
//    {
//        /// <summary>
//        /// Maps Exam entity to ExamDto
//        /// </summary>
//        public static ExamDto ToDto(Models.Exam exam)
//        {
//            if (exam == null) return null;

//            return new ExamDto
//            {
//                ID = exam.ID,
//                Title = exam.Title,
//                Date = exam.Date,
//                DurationInMinutes = exam.DurationInMinutes,
//                Fullmark = exam.Fullmark,
//                PassingScore = exam.PassingScore,
//                ExamType = exam.ExamType.ToString(),
//                QuestionsCount = exam.QuestionsCount,
//                CourseId = exam.CourseId,
//                CourseName = exam.Course?.Name ?? string.Empty,
//                InstructorId = exam.InstructorId,
//                InstructorName = exam.Instructor?.User != null
//                    ? $"{exam.Instructor.User.FirstName} {exam.Instructor.User.LastName}"
//                    : string.Empty,
//                CreatedAt = exam.CreatedAt
//            };
//        }

//        /// <summary>
//        /// Maps Exam entity to ExamDetailsDto with related data
//        /// </summary>
//        public static ExamDetailsDto ToDetailsDto(Models.Exam exam)
//        {
//            if (exam == null) return null;

//            return new ExamDetailsDto
//            {
//                ID = exam.ID,
//                Title = exam.Title,
//                Date = exam.Date,
//                DurationInMinutes = exam.DurationInMinutes,
//                Fullmark = exam.Fullmark,
//                PassingScore = exam.PassingScore,
//                ExamType = exam.ExamType.ToString(),
//                QuestionsCount = exam.QuestionsCount,
//                CreatedAt = exam.CreatedAt,

//                // Course Info
//                CourseId = exam.CourseId,
//                CourseName = exam.Course?.Name ?? string.Empty,
//                CourseDescription = exam.Course?.Description ?? string.Empty,

//                // Instructor Info
//                InstructorId = exam.InstructorId,
//                InstructorName = exam.Instructor?.User != null
//                    ? $"{exam.Instructor.User.FirstName} {exam.Instructor.User.LastName}"
//                    : string.Empty,
//                InstructorEmail = exam.Instructor?.User?.Email ?? string.Empty,

//                // Statistics
//                AssignedStudentsCount = exam.StudentExams?.Count ?? 0,
//                CompletedAttemptsCount = exam.ExamAttempts?.Count(a => a.EndTime.HasValue) ?? 0,
//                TotalQuestionsCount = exam.Questions?.Count ?? 0,

//                // Related Questions
//                Questions = exam.Questions?.Select(eq => new ExamQuestionDto
//                {
//                    QuestionId = eq.QuestionId,
//                    QuestionText = eq.Question?.QuestionText ?? string.Empty,
                   
//                    Points = eq.Question?.Points ?? 0
//                }).ToList() ?? new List<ExamQuestionDto>(),

//                // Assigned Students
//                AssignedStudents = exam.StudentExams?.Select(se => new AssignedStudentDto
//                {
//                    StudentId = se.StudentId,
//                    StudentName = se.Student?.User != null
//                        ? $"{se.Student.User.FirstName} {se.Student.User.LastName}"
//                        : string.Empty,
//                    Major = se.Student?.Major ?? string.Empty,
//                    HasAttempted = exam.ExamAttempts?.Any(ea => ea.StudentId == se.StudentId) ?? false,
//                    Score = exam.ExamAttempts?
//                        .Where(ea => ea.StudentId == se.StudentId && ea.Score.HasValue)
//                        .OrderByDescending(ea => ea.EndTime)
//                        .FirstOrDefault()?.Score
//                }).ToList() ?? new List<AssignedStudentDto>()
//            };
//        }

//        /// <summary>
//        /// Maps CreateExamDto to Exam entity
//        /// </summary>
//        public static Models.Exam ToEntity(CreateExamDto dto)
//        {
//            if (dto == null) return null;

//            return new Models.Exam
//            {
//                Title = dto.Title,
//                Date = dto.Date,
//                DurationInMinutes = dto.DurationInMinutes,
//                Fullmark = dto.Fullmark,
//                ExamType = (ExamType)dto.ExamType,
//                PassingScore = dto.PassingScore,
//                QuestionsCount = dto.QuestionsCount,
//                CourseId = dto.CourseId,
//                InstructorId = dto.InstructorId,
//                CreatedAt = DateTime.UtcNow,
//                IsDeleted = false
//            };
//        }

//        /// <summary>
//        /// Updates Exam entity from UpdateExamDto
//        /// </summary>
//        public static void UpdateEntity(Models.Exam exam, UpdateExamDto dto)
//        {
//            if (exam == null || dto == null) return;

//            if (!string.IsNullOrWhiteSpace(dto.Title))
//                exam.Title = dto.Title;

//            if (dto.Date.HasValue)
//                exam.Date = dto.Date.Value;

//            if (dto.DurationInMinutes.HasValue && dto.DurationInMinutes.Value > 0)
//                exam.DurationInMinutes = dto.DurationInMinutes.Value;

//            if (dto.Fullmark.HasValue && dto.Fullmark.Value > 0)
//                exam.Fullmark = dto.Fullmark.Value;

//            if (dto.ExamType.HasValue)
//                exam.ExamType = (ExamType)dto.ExamType.Value;

//            if (dto.PassingScore.HasValue)
//                exam.PassingScore = dto.PassingScore.Value;

//            if (dto.QuestionsCount.HasValue && dto.QuestionsCount.Value > 0)
//                exam.QuestionsCount = dto.QuestionsCount.Value;

//            if (dto.CourseId.HasValue && dto.CourseId.Value > 0)
//                exam.CourseId = dto.CourseId.Value;

//            if (dto.InstructorId.HasValue && dto.InstructorId.Value > 0)
//                exam.InstructorId = dto.InstructorId.Value;
//        }

//        /// <summary>
//        /// Maps list of Exam entities to ExamDto list
//        /// </summary>
//        public static IEnumerable<ExamDto> ToDtoList(IEnumerable<Models.Exam> exams)
//        {
//            return exams?.Select(ToDto).Where(dto => dto != null) ?? Enumerable.Empty<ExamDto>();
//        }
//    }
//}