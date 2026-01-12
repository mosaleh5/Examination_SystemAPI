namespace Examination_System.DTOs.Student
{
    public class StudentDetailsDto
    {
        public Guid Id { get; set; }  // Changed from string
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}