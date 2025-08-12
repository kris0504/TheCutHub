using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Models;

namespace TheCutHub.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<Barber> Barbers { get; set; }
		public DbSet<Service> Services { get; set; }
		public DbSet<Appointment> Appointments { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<WorkingHour> WorkingHours { get; set; }
		public DbSet<WorkImage> WorkImages { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);


			builder.Entity<Service>().HasData(
				new Service { Id = 1, Name = "Haircut", Description = "Classic haircut", Price = 25, DurationMinutes = 30 },
				new Service { Id = 2, Name = "Beard Trim", Description = "Beard shaping", Price = 15, DurationMinutes = 20 },
				new Service { Id = 3, Name = "Combo", Description = "Haircut + beard", Price = 35, DurationMinutes = 45 }
			);


			builder.Entity<Barber>()
				.HasOne(b => b.User)
				.WithMany()
				.HasForeignKey(b => b.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Service>()
				.Property(s => s.Price)
				.HasPrecision(10, 2);

			builder.Entity<WorkingHour>()
				.HasIndex(w => new { w.BarberId, w.Day })
				.IsUnique();


			builder.Entity<WorkingHour>()
				.Property(w => w.SlotIntervalInMinutes)
				.HasDefaultValue(30);

		}
	}
}
