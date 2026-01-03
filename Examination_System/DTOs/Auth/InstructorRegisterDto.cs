namespace Examination_System.DTOs.Auth
{
    public class InstructorRegisterDto : RegisterDto
    {
        public string Department { get; set; }
        public string Specialization { get; set; }
    }
}
