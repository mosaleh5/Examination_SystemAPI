using System.Security.Claims;

namespace Examination_System.Services.CurrentUserServices
{
    public interface ICurrentUserServices
    {     
            Guid UserId { get; }     
            string? UserName { get; }
            bool IsInRole(string roleName);           
            bool IsAuthenticated { get; }
      
    }
}