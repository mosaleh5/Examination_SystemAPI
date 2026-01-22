namespace Examination_System.ViewModels.Choice
{
    public class UpdateChoiceViewModel
    {
        public Guid Id { get; set; }  
        public string? Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
