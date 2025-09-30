using Backend.Domain.Models;
using Backend.Utility;
using Microsoft.AspNetCore.Identity;

namespace Backend.Persistence.Data
{

    /// <summary>
    /// This class is responsible for making sure the required roles
    /// (like "Admin" and "User") exist in the database.
    /// ASP.NET Identity does not create roles automatically,
    /// so we have to "seed" them at application startup.
    /// It also creates a default admin account.
    /// </summary>


    public static class RoleAndAdminSeeder
    {
        /// <summary>
        /// Create roles in the database if they do not already exist.
        /// </summary>

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // The roles we want in our system
            string[] roleNames = { Roles.Admin, Roles.User };

            foreach (var roleName in roleNames)
            {
                // Check if the role already exists in AspNetRoles table
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                // If the role is missing, create it
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }


            }

        }

        /// <summary>
        /// Create a default admin account the first time the app runs.
        /// </summary>

        public static async Task SeedAdminAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure roles are created first
            await SeedRolesAsync(roleManager);

            // Define default admin email and password
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? throw new Exception("Admin email is missing");
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? throw new Exception("Admin password is missing");


            // Check if an admin user already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Create new admin user
                var admin = new AppUser
                {
                    UserName = "heidi",
                    Email = adminEmail,
                    CreatedAt = DateTime.UtcNow,

                };

                // Create user with a default password
                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    // Add admin role to the new user
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
                }
            }
        }

    }

}
