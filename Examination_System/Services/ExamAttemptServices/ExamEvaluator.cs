using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;

namespace Examination_System.Services.ExamAttemptServices
{
    public interface IExamEvaluator
    {
        Task<ExamEvaluationResult> EvaluateAsync(ExamAttempt attempt, List<SubmitAnswerDto> answers, List<Question> questions);
    }

    public class ExamEvaluator : IExamEvaluator
    {
        public async Task<ExamEvaluationResult> EvaluateAsync(ExamAttempt attempt, List<SubmitAnswerDto> answers, List<Question> questions)
        {
            var studentAnswers = new List<StudentAnswer>();
            var totalScore = 0;
            var maxScore = 0;

            foreach (var answer in answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null) continue;

                var studentAnswer = CreateStudentAnswer(answer, question, attempt.Id);
                studentAnswers.Add(studentAnswer);

                if (studentAnswer.IsCorrect)
                {
                    totalScore += question.Mark;
                }

                maxScore += question.Mark;
            }

            var percentage = maxScore > 0 ? ((double)totalScore / maxScore) * 100 : 0;

            return new ExamEvaluationResult
            {
                StudentAnswers = studentAnswers,
                TotalScore = totalScore,
                MaxScore = maxScore,
                Percentage = percentage
            };
        }

        private StudentAnswer CreateStudentAnswer(SubmitAnswerDto answer, Question question, Guid attemptId)
        {
            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            var isCorrect = correctChoice != null && correctChoice.Id == answer.ChoiceId;

            return new StudentAnswer
            {
                QuestionId = question.Id,
                SelectedChoiceId = answer.ChoiceId,
                AttemptId = attemptId,
                IsCorrect = isCorrect
            };
        }
    }

    public class ExamEvaluationResult
    {
        public List<StudentAnswer> StudentAnswers { get; set; } = [];
        public int TotalScore { get; set; }
        public int MaxScore { get; set; }
        public double Percentage { get; set; }
    }
}