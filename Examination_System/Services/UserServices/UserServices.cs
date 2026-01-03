using Examination_System.DTOs.Auth;
using Examination_System.Models;
using Microsoft.AspNetCore.Identity;
using Examination_System.Services.TokenServices;
using Examination_System.Repository.UnitOfWork;
namespace Examination_System.Services.UserServices
{
    public class UserServices : IUserServices
    {
        private readonly ITokenServices _tokenServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserServices(UserManager<User> userManager ,
            SignInManager<User> signInManager ,
            ITokenServices tokenServices,
            IUnitOfWork UnitOfWork)  
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenServices = tokenServices;
            _unitOfWork = UnitOfWork;
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<UserDto?> LoginAsync(LoginDto loginDto)
        {
            var user = _userManager.FindByEmailAsync(loginDto.Email).Result;
            if (user is null) return null;
            var result = _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false).Result;
            if (!result.Succeeded) return null;
            //var roles = _userManager.GetRolesAsync(user).Result;
            var role = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                // Role = roles.FirstOrDefault() ?? string.Empty,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager, role),
            };

        }

        public async Task<UserDto?> RegisterInstructorAsync(InstructorRegisterDto registerDto)
        {

            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await RegisterUserAsync(registerDto);
                if (user is null) return null;

                var foundUser = await _userManager.FindByIdAsync(user);
                if (foundUser is not null)
                {
                    await _userManager.AddToRoleAsync(foundUser, "Instructor");
                }
                var instructor = new Instructor
                {
                    Id = user,
                    Department = registerDto.Department,
                    Specialization = registerDto.Specialization,
                };
             
                await _unitOfWork.Repository<Instructor, string>().AddAsync(instructor);
                var result = await _unitOfWork.CompleteAsync();
              
                await transaction.CommitAsync();
                var role = await _userManager.GetRolesAsync(foundUser);
                return new UserDto
                {
                    Id = instructor.Id,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    Token = await _tokenServices.CreateTokenAsync(
                        await _userManager.FindByIdAsync(instructor.Id), _userManager , role),
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to register student", ex);
            }
        }

        public async Task<UserDto?> RegisterStudentAsync(StudentRegisterDto registerDto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try 
            {
                var user = await RegisterUserAsync(registerDto);
                if (user is null) return null;
                
                var foundUser = await _userManager.FindByIdAsync(user);
                if (foundUser is not null)
                {
                    await _userManager.AddToRoleAsync(foundUser, "Student");
                }
                
                var student = new Student
                {
                    Id = user,
                    Major = registerDto.Major,
                };
                await _unitOfWork.Repository<Student, string>().AddAsync(student);
                var result = await _unitOfWork.CompleteAsync();
                
                await transaction.CommitAsync();
                var role = await _userManager.GetRolesAsync(foundUser);

                return new UserDto
                {
                    Id = student.Id,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    Token = await _tokenServices.CreateTokenAsync(
                        await _userManager.FindByIdAsync(student.Id), _userManager , role),
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to register student", ex);
            }
        }

       

        public async Task<string> RegisterUserAsync(RegisterDto registerDto)
        {
            if (await IsEmailExistAsync(registerDto.Email)) 
            {
                throw new Exception("Email already exists");
            }
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
                throw new Exception("Failed to register user");
            }
            return user.Id;
        }

      
    }
}
