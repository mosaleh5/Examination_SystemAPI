namespace Examination_System.DTOs.Auth
{
    public class InstructorRegisterDto : RegisterDto
    {
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
    }
}
