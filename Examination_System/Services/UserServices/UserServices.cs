using Examination_System.Common;
using Examination_System.DTOs.Auth;
using Examination_System.Models;
using Examination_System.Models.Enums;
using Examination_System.Repository.UnitOfWork;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Examination_System.Services.TokenServices;
namespace Examination_System.Services.UserServices
{
    public class UserServices : IUserServices
    {
        private readonly ITokenServices _tokenServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<StudentRegisterDto> _studentRegisterValidator;
        private readonly IValidator<InstructorRegisterDto> _instructorRegisterValidator;

        public UserServices(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenServices tokenServices,
            IUnitOfWork unitOfWork,
            IValidator<LoginDto> loginValidator,
            IValidator<StudentRegisterDto> studentRegisterValidator,
            IValidator<InstructorRegisterDto> instructorRegisterValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenServices = tokenServices;
            _unitOfWork = unitOfWork;
            _loginValidator = loginValidator;
            _studentRegisterValidator = studentRegisterValidator;
            _instructorRegisterValidator = instructorRegisterValidator;
        }

        public Task<bool> IsEmailExistAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                return Result<UserDto>.ValidaitonFailure(validationResult);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Result<UserDto>.Failure(
                    ErrorCode.NotFound,
                    "Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Result<UserDto>.Failure(
                    ErrorCode.Unauthorized,
                    "Invalid email or password");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager, roles),
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            return Result<UserDto>.Success(userDto);
        }

        public async Task<Result<UserDto>> RegisterInstructorAsync(InstructorRegisterDto registerDto)
        {
            var validationResult = await _instructorRegisterValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                return Result<UserDto>.ValidaitonFailure(validationResult);
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                PhoneNumber = registerDto.Phone,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return Result<UserDto>.Failure(
                    ErrorCode.BadRequest,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "Instructor");

            var instructor = new Instructor
            {
                Id = user.Id,
                Department = registerDto.Department,
                Specialization = registerDto.Specialization,
            };

            await _unitOfWork.Repository<Instructor>().AddAsync(instructor);
            var rowsAffected = await _unitOfWork.CompleteAsync();

            if (rowsAffected == null || rowsAffected < 1)
            {
                await transaction.RollbackAsync();
                return Result<UserDto>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to create instructor record");
            }

            await transaction.CommitAsync();

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager, roles),
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            return Result<UserDto>.Success(userDto);
        }

        public async Task<Result<UserDto>> RegisterStudentAsync(StudentRegisterDto registerDto)
        {
            var validationResult = await _studentRegisterValidator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
            {
                return Result<UserDto>.ValidaitonFailure(validationResult);
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                PhoneNumber = registerDto.Phone,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return Result<UserDto>.Failure(
                    ErrorCode.BadRequest,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "Student");

            var student = new Student
            {
                Id = user.Id,
                Major = registerDto.Major,
            };

            await _unitOfWork.Repository<Student>().AddAsync(student);
            var rowsAffected = await _unitOfWork.CompleteAsync();

            if (rowsAffected == null || rowsAffected < 1)
            {
                await transaction.RollbackAsync();
                return Result<UserDto>.Failure(
                    ErrorCode.DatabaseError,
                    "Failed to create student record");
            }

            await transaction.CommitAsync();

            var roles = await _userManager.GetRolesAsync(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager, roles),
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            return Result<UserDto>.Success(userDto);
        }

        public Task<string> RegisterUserAsync(RegisterDto registerDto)
        {
            throw new NotImplementedException();
        }
    }
}
