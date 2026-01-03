using Examination_System.Models;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Services.TokenServices
{
    public interface ITokenServices
    {
        Task<string > CreateTokenAsync(User user, UserManager<User> userManager ,IList<string> role);
    }
}
