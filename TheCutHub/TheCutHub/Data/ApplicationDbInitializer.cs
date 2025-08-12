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

			
			await ctx.Database.MigrateAsync();

			
			foreach (var r in new[] { "Administrator", "Barber", "User" })
				if (!await roleMgr.RoleExistsAsync(r))
					await roleMgr.CreateAsync(new IdentityRole(r));


			var admin = await userMgr.FindByEmailAsync(AdminEmail);
			if (admin == null)
			{
				admin = new ApplicationUser { UserName = AdminEmail, Email = AdminEmail, EmailConfirmed = true, FullName = "Site Administrator" };
				await userMgr.CreateAsync(admin, AdminPassword);
			}
			if (!await userMgr.IsInRoleAsync(admin, "Administrator"))
				await userMgr.AddToRoleAsync(admin, "Administrator");

			
			var barberUser = await userMgr.FindByEmailAsync(BarberEmail);
			if (barberUser == null)
			{
				barberUser = new ApplicationUser { UserName = BarberEmail, Email = BarberEmail, EmailConfirmed = true, FullName = "John Blade" };
				await userMgr.CreateAsync(barberUser, BarberPassword);
			}
			if (!await userMgr.IsInRoleAsync(barberUser, "Barber"))
				await userMgr.AddToRoleAsync(barberUser, "Barber");

			
			var barber = await ctx.Barbers.FirstOrDefaultAsync(b => b.UserId == barberUser.Id);
			if (barber == null)
			{
				barber = new Barber
				{
					UserId = barberUser.Id,
					FullName = "John Blade",
					Bio = "Fade specialist",
					ProfileImageUrl = "/images/default-barber.png"
				};
				ctx.Barbers.Add(barber);
				await ctx.SaveChangesAsync();
			}

			
			var allBarbers = await ctx.Barbers.Select(b => b.Id).ToListAsync();
			foreach (var barberId in allBarbers)
			{
				foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
				{
					var exists = await ctx.WorkingHours.AnyAsync(w => w.BarberId == barberId && w.Day == day);
					if (!exists)
					{
						var isWeekend = day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
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
		}
	}
}
