using System.ComponentModel.DataAnnotations;

namespace Examination_System.DTOs.Course
{
    public class CreateCourseDto
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

        [Required(ErrorMessage = "Instructor ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid instructor ID")]
        public int InstructorId { get; set; }
    }
}
