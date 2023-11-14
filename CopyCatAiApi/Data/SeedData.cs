// Purpose: Contains seed data for the database.

using Microsoft.AspNetCore.Identity;

namespace CopyCatAiApi.Data
{
    public static class SeedData
    {
        public static async Task LoadRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                var admin = new IdentityRole { Name = "admin", NormalizedName = "ADMIN" };
                var user = new IdentityRole { Name = "user", NormalizedName = "USER" };

                await roleManager.CreateAsync(admin);
                await roleManager.CreateAsync(user);
            }
        }
    }
}