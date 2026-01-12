using Examination_System.DTOs.Auth;
using Examination_System.Common;
namespace Examination_System.Services.UserServices
{
    public interface IUserServices
    {
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto);
        Task<Result<UserDto>> RegisterStudentAsync(StudentRegisterDto registerDto);
        Task<Result<UserDto>> RegisterInstructorAsync(InstructorRegisterDto registerDto);
        Task<string> RegisterUserAsync(RegisterDto registerDto);
        Task<bool> IsEmailExistAsync(string email);
    }

    
}
