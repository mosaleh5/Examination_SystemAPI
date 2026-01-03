namespace Examination_System.DTOs.ExamAttempt
{
    public class resultToReturnDto
    {
        public DateTime TimeToFinish { get; set; }
        public  int TotalScore { get; set; }
        public double scorePercentage { get; set; }
        public bool isSuccessed { get; set; }
        public List<AttemptAnswersDto> Answers { get; set; }


    }
}
