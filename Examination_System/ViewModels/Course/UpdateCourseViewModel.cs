using System.ComponentModel.DataAnnotations;

namespace Examination_System.ViewModels.Course
{
    public class UpdateCourseViewModel
    {
        public Guid ID { get; set; }  
        
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Course name must be between 3 and 100 characters")]
        public string? Name { get; set; } = string.Empty;

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters")]
        public string? Description { get; set; } = string.Empty;

        [RegularExpression(@"^\d+$", ErrorMessage = "Hours must be a valid number")]
        public string? Hours { get; set; } = string.Empty;

        //[Range(1, int.MaxValue, ErrorMessage = "Invalid instructor ID")]
        public Guid? InstructorId { get; set; }
    }
}