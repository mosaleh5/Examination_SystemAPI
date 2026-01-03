using Examination_System.Models;
using Microsoft.AspNetCore.Identity;

namespace Examination_System.Data.SeedData
{
    public class UserDataSeeding
    {
        public async static Task UserSeedingAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await RoleSeedingAsync(roleManager);
            await AdminUserSeedingAsync(userManager);
        }

        private async static Task RoleSeedingAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "Instructor", "Student" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                  
                }
            }
        }

        private async static Task AdminUserSeedingAsync(UserManager<User> userManager)
        {
            if (!userManager.Users.Any())
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@123",
                    PhoneNumber = "1234567890",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
