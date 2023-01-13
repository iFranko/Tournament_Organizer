using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tournament_Organizer.Models;

namespace Tournament_Organizer.Data
{
    public static class DbInitializer
    {
        
        public static async Task<int> SeedUsersAndRoles( IServiceProvider serviceProvider)

        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var context = serviceProvider.GetRequiredService<TournamentOrganizerContext>();
            context.Database.Migrate();

            if (roleManager.Roles.Count() > 0)
                return 1;  // should log an error message here

            // Seed roles
            int result = await SeedRoles(roleManager);
            if (result != 0)
                return 2;  // should log an error message here


            // Check if users already exist and exit if there are
            if (userManager.Users.Count() > 0)
                return 3;  // should log an error message here

            // Seed users
            result = await SeedUsers(userManager);
            if (result != 0)
                return 4;  // should log an error message here



            return 0;

            

       
        }

        private static async Task<int> SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Create Admin Role
            var result = await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!result.Succeeded)
                return 1;  // should log an error message here

            // Create Ogranizer Role
            result = await roleManager.CreateAsync(new IdentityRole("Organizer"));
            if (!result.Succeeded)
                return 2;  // should log an error message here

            // Create Player Role
            result = await roleManager.CreateAsync(new IdentityRole("Player"));
            if (!result.Succeeded)
                return 3;  // should log an error message here

            result = await roleManager.CreateAsync(new IdentityRole("Manager"));
            if (!result.Succeeded)
                return 4;  // should log an error message here

            return 0;

        }

        private static async Task<int> SeedUsers(UserManager<User> userManager)
        {
            // Create Admin User
            var adminUser = new User
            {
                UserName = "Admin",
                Email = "Admin@mohawkcollege.ca",
                First_Name = "Admin",
                Last_Name = "Admin",
                EmailConfirmed = true,
                User_Role = "Admin"
            };
            var result = await userManager.CreateAsync(adminUser, "AdminPass!1");
            if (!result.Succeeded)
                return 1;  // should log an error message here

            // Assign user to Manager role
            result = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (!result.Succeeded)
                return 2;  // should log an error message here



            return 0;
        }

    }
}
