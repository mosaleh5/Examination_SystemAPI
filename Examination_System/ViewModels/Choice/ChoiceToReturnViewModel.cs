namespace Examination_System.ViewModels.Choice
{
    public class ChoiceToReturnViewModel
    {
        public Guid Id {get; set; }
        public string Text { get; set; }

        public bool IsCorrect { get; set; }

        public Guid QuestionId { get; set; }
    }
}
