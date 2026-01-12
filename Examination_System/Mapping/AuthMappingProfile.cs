using AutoMapper;
using Examination_System.DTOs.Auth;
using Examination_System.ViewModels.Auth;

namespace Examination_System.Mapping
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<LoginViewModel, LoginDto>();
            CreateMap<StudentRegisterViewModel, StudentRegisterDto>();
            CreateMap<InstructorRegisterViewModel, InstructorRegisterDto>();
        }
    }
}