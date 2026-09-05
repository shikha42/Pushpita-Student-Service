using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Student", "Staff" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Clean up legacy staff seeds (aranna@iubat.edu, shakkhorpaul50@gmail.com) if they exist
            foreach (var legacyEmail in new[] { "aranna@iubat.edu", "shakkhorpaul50@gmail.com" })
            {
                var legacy = await userManager.FindByEmailAsync(legacyEmail);
                if (legacy != null)
                {
                    await userManager.DeleteAsync(legacy);
                }
            }

            string staffEmail = "pushpita@iubat.edu";
            if (await userManager.FindByEmailAsync(staffEmail) == null)
            {
                var staffUser = new ApplicationUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    FirstName = "Pushpita",
                    LastName = "IUBAT",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(staffUser, "P@ssW0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(staffUser, "Staff");
                }
            }
            else
            {
                // Ensure existing pushpita has Staff role and correct password
                var existing = await userManager.FindByEmailAsync(staffEmail);
                if (existing != null)
                {
                    if (!await userManager.IsInRoleAsync(existing, "Staff"))
                        await userManager.AddToRoleAsync(existing, "Staff");
                    // Reset password to P@ssW0rd if needed (idempotent check)
                    var token = await userManager.GeneratePasswordResetTokenAsync(existing);
                    await userManager.ResetPasswordAsync(existing, token, "P@ssW0rd");
                }
            }
        }
    }
}
