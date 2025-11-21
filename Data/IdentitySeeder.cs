using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedRolesAndAdmin(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seeding logic
            string[] roles = { "Lecturer", "Coordinator", "Manager", "HR" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Default Admin / Manager
            var managerEmail = "manager@college.com";
            var manager = await userManager.FindByEmailAsync(managerEmail);

            if (manager == null)
            {
               var defaultManager = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(defaultManager, "Password123!");
                await userManager.AddToRoleAsync(defaultManager, "Manager");
            }
        }
    }
}
