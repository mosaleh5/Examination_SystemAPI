using Examination_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.ViewModels.Choice
{
    public class ChoiceViewModel
    {

        public Guid Id { get; set; }  // Changed from int
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }

    }
}
