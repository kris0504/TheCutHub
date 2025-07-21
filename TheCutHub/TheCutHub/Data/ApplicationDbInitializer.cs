using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Models;

namespace TheCutHub.Data
{
    public static class ApplicationDbInitializer
    {
        private const string AdminEmail = "admin@barberbook.com";
        private const string AdminPassword = "Admin123!";
        public static async Task SeedWorkingHoursAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var barbers = await context.Barbers.ToListAsync();
            var existingHours = await context.WorkingHours.ToListAsync();

            foreach (var barber in barbers)
            {
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (!existingHours.Any(wh => wh.BarberId == barber.Id && wh.Day == day))
                    {
                        context.WorkingHours.Add(new WorkingHour
                        {
                            BarberId = barber.Id,
                            Day = day,
                            StartTime = new TimeSpan(9, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            IsWorking = true
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            // Seed roles
            string[] roles = { "Administrator", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed admin user
            if (await userManager.FindByEmailAsync(AdminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    FullName = "Site Administrator"
                };

                var result = await userManager.CreateAsync(admin, AdminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Administrator");
                }
            }
        }
    }
}
