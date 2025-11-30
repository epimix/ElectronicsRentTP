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
                var dbContext = scope.ServiceProvider.GetRequiredService<DataAccess.Data.EquipmentRentalDbContext>();

                try
                {
                    // Перевіряємо, чи можемо підключитися до бази перед seeding
                    bool canConnect = false;
                    try
                    {
                        canConnect = dbContext.Database.CanConnect();
                    }
                    catch (Exception connectEx)
                    {
                        logger.LogWarning(connectEx, "Cannot connect to database. Connection string might be incorrect. Error: {Error}", connectEx.Message);
                        logger.LogWarning("Skipping roles and admin seeding. Please check your connection string in Azure App Service Configuration.");
                        logger.LogWarning("Expected connection string format: Server=EqRentFtp.mssql.somee.com;Database=EqRentFtp;User Id=...;Password=...;TrustServerCertificate=True;");
                        return;
                    }

                    if (!canConnect)
                    {
                        logger.LogWarning("Cannot connect to database. Skipping roles and admin seeding. Please check your connection string in Azure App Service Configuration.");
                        return;
                    }

                    IdentityInitializer.SeedRolesAsync(roleManager).Wait();
                    IdentityInitializer.SeedAdminAsync(userManager).Wait();
                    logger.LogInformation("Roles and admin user seeded successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to seed roles and admin user. This might be due to database connection issues. Error: {ErrorMessage}", ex.Message);
                    logger.LogWarning("Application will continue to start, but database operations may fail until connection string is fixed.");
                    // Don't throw - allow the app to start even if seeding fails
                    // The database might not be ready yet or connection string might be wrong
                }
            }
        }
    }
}
