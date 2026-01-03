using Examination_System.DTOs.Auth;

namespace Examination_System.Services.UserServices
{
    public interface IUserServices
    {
        Task<UserDto> LoginAsync(LoginDto loginDto);
        Task<UserDto> RegisterStudentAsync(StudentRegisterDto registerDto);
        Task<UserDto> RegisterInstructorAsync(InstructorRegisterDto registerDto);
        Task<string> RegisterUserAsync(RegisterDto registerDto);
        Task<bool> IsEmailExistAsync(string email);
    }

    
}
