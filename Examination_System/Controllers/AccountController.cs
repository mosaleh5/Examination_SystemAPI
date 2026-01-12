using Examination_System.DTOs.Auth;
using Examination_System.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using Examination_System.Common;
using Examination_System.ViewModels;
using Examination_System.Models.Enums;
namespace Examination_System.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserServices _userService;
        
        public AccountController(IUserServices userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseViewModel<UserDto>), 200)]
        [ProducesResponseType(typeof(ResponseViewModel<UserDto>), 401)]
        public async Task<ActionResult<ResponseViewModel<UserDto>>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<UserDto>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            var result = await _userService.LoginAsync(loginDto);
            
            return result.IsSuccess 
                ? Ok(ResponseViewModel<UserDto>.Success(result.Data, "Login successful"))
                : Unauthorized(ResponseViewModel<UserDto>.Failure(result.Error, result.ErrorMessage));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseViewModel<UserDto>), 201)]
        [ProducesResponseType(typeof(ResponseViewModel<UserDto>), 400)]
        public async Task<ActionResult<ResponseViewModel<UserDto>>> RegisterStudent([FromBody] StudentRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<UserDto>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            var result = await _userService.RegisterStudentAsync(registerDto);
            
            return result.IsSuccess 
                ? CreatedAtAction(nameof(Login), ResponseViewModel<UserDto>.Success(result.Data, "Student registered successfully"))
                : BadRequest(ResponseViewModel<UserDto>.Failure(result.Error, "Registration failed: " + result.ErrorMessage));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseViewModel<UserDto>), 201)]
        [ProducesResponseType(typeof(ResponseViewModel<UserDto>), 400)]
        public async Task<ActionResult<ResponseViewModel<UserDto>>> RegisterInstructor([FromBody] InstructorRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseViewModel<UserDto>.Failure(
                    ErrorCode.ValidationError,
                    GetValidationErrors()));

            var result = await _userService.RegisterInstructorAsync(registerDto);
            
            return result.IsSuccess 
                ? CreatedAtAction(nameof(Login), ResponseViewModel<UserDto>.Success(result.Data, "Instructor registered successfully"))
                : BadRequest(ResponseViewModel<UserDto>.Failure(result.Error, "Registration failed: " + result.ErrorMessage));
        }
    }
}
