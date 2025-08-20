using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Models;

namespace TheCutHub.Data
{
    public static class ApplicationDbInitializer
    {
        private const string AdminEmail = "admin@barberbook.com";
        private const string AdminPassword = "Admin123!";
        private const string BarberEmail = "barber1@barberbook.com";
        private const string BarberPassword = "Barber123!";

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            
            var provider = ctx.Database.ProviderName ?? string.Empty; 
            if (provider.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                await ctx.Database.MigrateAsync();
            }
            else
            {
                await ctx.Database.EnsureCreatedAsync();
            }

            foreach (var role in new[] { "Administrator", "Barber", "User" })
            {
                if (!await roleMgr.RoleExistsAsync(role))
                    _ = await roleMgr.CreateAsync(new IdentityRole(role));
            }

            var admin = await userMgr.FindByEmailAsync(AdminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    FullName = "Site Administrator"
                };
                var created = await userMgr.CreateAsync(admin, AdminPassword);
              
            }
            if (admin != null && !await userMgr.IsInRoleAsync(admin, "Administrator"))
            {
                _ = await userMgr.AddToRoleAsync(admin, "Administrator");
            }

            
            var barberUser = await userMgr.FindByEmailAsync(BarberEmail);
            if (barberUser == null)
            {
                barberUser = new ApplicationUser
                {
                    UserName = BarberEmail,
                    Email = BarberEmail,
                    EmailConfirmed = true,
                    FullName = "John Blade"
                };
                var created = await userMgr.CreateAsync(barberUser, BarberPassword);
            }
            if (barberUser != null && !await userMgr.IsInRoleAsync(barberUser, "Barber"))
            {
                _ = await userMgr.AddToRoleAsync(barberUser, "Barber");
            }

            
            if (barberUser != null)
            {
                var existingBarber = await ctx.Barbers.FirstOrDefaultAsync(b => b.UserId == barberUser.Id);
                if (existingBarber == null)
                {
                    existingBarber = new Barber
                    {
                        UserId = barberUser.Id,
                        FullName = "John Blade",
                        Bio = "Fade specialist",
                        ProfileImageUrl = "/images/default-barber.png"
                    };
                    ctx.Barbers.Add(existingBarber);
                    await ctx.SaveChangesAsync();
                }
            }

            
            var allBarberIds = await ctx.Barbers.Select(b => b.Id).ToListAsync();
            foreach (var barberId in allBarberIds)
            {
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    bool exists = await ctx.WorkingHours.AnyAsync(w => w.BarberId == barberId && w.Day == day);
                    if (!exists)
                    {
                        bool isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
                        ctx.WorkingHours.Add(new WorkingHour
                        {
                            BarberId = barberId,
                            Day = day,
                            StartTime = new TimeSpan(9, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            IsWorking = !isWeekend,
                            SlotIntervalInMinutes = 30
                        });
                    }
                }
            }

            await ctx.SaveChangesAsync();

            if (!await ctx.Services.AnyAsync())
            {
                ctx.Services.AddRange(
                    new Service { Name = "Haircut", Description = "Classic haircut", DurationMinutes = 30, Price = 25.00m },
                    new Service { Name = "Beard Trim", Description = "Beard shaping", DurationMinutes = 20, Price = 15.00m },
                    new Service { Name = "Combo", Description = "Haircut + beard", DurationMinutes = 45, Price = 35.00m }
                );
                await ctx.SaveChangesAsync();
            }
        }
    }
}
