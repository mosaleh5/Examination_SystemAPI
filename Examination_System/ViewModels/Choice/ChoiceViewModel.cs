using Examination_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examination_System.ViewModels.Choice
{
    public class ChoiceViewModel
    {


        [Required]
        public string Text { get; set; }
        [Required]

        public bool IsCorrect { get; set; }     

    }
}
