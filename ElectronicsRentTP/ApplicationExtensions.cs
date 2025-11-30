using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    IdentityInitializer.SeedRolesAsync(roleManager).Wait();
                    IdentityInitializer.SeedAdminAsync(userManager).Wait();
                    logger.LogInformation("Roles and admin user seeded successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to seed roles and admin user. This might be due to database connection issues.");
                    // Don't throw - allow the app to start even if seeding fails
                    // The database might not be ready yet or connection string might be wrong
                }
            }
        }
    }
}
