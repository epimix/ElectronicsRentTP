using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace ElectronicsRentTP.Helpers
{
    public static class Roles
    {
        public const string ADMIN = "admin";
        public const string USER = "user";
    }

    public static class IdentityInitializer
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //var roleManager = app.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(Roles.ADMIN))
                await roleManager.CreateAsync(new(Roles.ADMIN));

            if (!await roleManager.RoleExistsAsync(Roles.USER))
                await roleManager.CreateAsync(new(Roles.USER));
        }

        public static async Task SeedAdminAsync(UserManager<User> userManager)
        {
            //var userManager = app.GetRequiredService<UserManager<User>>();

            const string USERNAME = "admin@ukr.net";
            const string PASSWORD = "Qwer-1234";

            var existingUser = await userManager.FindByNameAsync(USERNAME);

            if (existingUser == null)
            {
                var user = new User
                {
                    UserName = USERNAME,
                    Email = USERNAME,
                };

                var result = await userManager.CreateAsync(user, PASSWORD);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, Roles.ADMIN);
            }
        }
    }
}
