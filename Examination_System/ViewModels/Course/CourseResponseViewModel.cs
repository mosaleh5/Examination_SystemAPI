namespace Examination_System.ViewModels.Course
{
    public class CourseResponseViewModel
    {
        public Guid ID { get; set; }  
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Hours { get; set; } = string.Empty;
        public Guid InstructorId { get; set; }  
        public string InstructorName { get; set; } = string.Empty;
    }
}
