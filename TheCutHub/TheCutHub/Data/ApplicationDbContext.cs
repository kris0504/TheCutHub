using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Models;

namespace TheCutHub.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<WorkImage> WorkImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Barber>()
        .HasOne(b => b.User)
        .WithMany() // или .WithMany(u => u.Barbers) ако имаш обратна колекция
        .HasForeignKey(b => b.UserId)
        .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkingHour>().HasData(
    Enum.GetValues(typeof(DayOfWeek))
        .Cast<DayOfWeek>()
        .Select((day, index) => new WorkingHour
        {
            Id = index + 1,
            Day = day,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            IsWorking = true,
            BarberId = 1
        })
);

        }
    }
}
