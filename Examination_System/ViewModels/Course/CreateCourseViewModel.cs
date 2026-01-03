using System.ComponentModel.DataAnnotations;

namespace Examination_System.ViewModels.Course
{
    public class CreateCourseViewModel
    {
        [Required(ErrorMessage = "Course name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Course name must be between 3 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Hours are required")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Hours must be a valid number")]
        public string Hours { get; set; }
    }
}
