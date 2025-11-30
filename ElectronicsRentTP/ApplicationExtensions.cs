using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace ElectronicsRentTP.Helpers
{
    public static class ApplicationExtensions
    {
        public static void SeedRolesAndAdmin(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                IdentityInitializer.SeedRolesAsync(roleManager).Wait();
                IdentityInitializer.SeedAdminAsync(userManager).Wait();
            }
        }
    }
}
