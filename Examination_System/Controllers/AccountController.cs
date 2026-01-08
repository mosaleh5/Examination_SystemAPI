using Examination_System.DTOs.Auth;
using Examination_System.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Controllers
{
    public class AccountController:BaseController
    {
        private readonly IUserServices _userService;
        public AccountController(IUserServices userService)
        {
            _userService = userService;
        }
        // Action methods for account management (e.g., Login, Register, Logout) would go here
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var userDto = await _userService.LoginAsync(loginDto);
            if (userDto == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterDto registerDto)
        {
            var userDto = await _userService.RegisterStudentAsync(registerDto);
            if (userDto == null)
            {
                return BadRequest("Registration failed.");
            }
            return Ok(userDto);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterInstructor([FromBody] InstructorRegisterDto registerDto)
        {
            var userDto = await _userService.RegisterInstructorAsync(registerDto);
            if (userDto == null)
            {
                return BadRequest("Registration failed.");
            }
            return Ok(userDto);
        }
    }
}
