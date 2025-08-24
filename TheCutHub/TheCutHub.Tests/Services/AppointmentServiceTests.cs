using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Services;
using Xunit;

namespace TheCutHub.Tests.Services
{
    public class AppointmentServiceTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_Should_Add_Appointment()
        {
            var context = GetInMemoryContext();
            var service = new AppointmentService(context);

            var appointment = new Appointment
            {
                Id = 1,
                UserId = "user1",
                Date = DateOnly.FromDateTime(DateTime.Today),
                TimeSlot = TimeSpan.FromHours(9)
            };

            await service.CreateAsync(appointment);
            var result = await context.Appointments.FirstOrDefaultAsync();

            Assert.NotNull(result);
            Assert.Equal("user1", result?.UserId);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Appointment_If_Owner()
        {
            var context = GetInMemoryContext();
            var service = new AppointmentService(context);

            context.Appointments.Add(new Appointment
            {
                Id = 1,
                UserId = "user1",
                Date = DateOnly.FromDateTime(DateTime.Today),
                TimeSlot = TimeSpan.FromHours(9)
            });
            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(1, "user1");

            Assert.True(result);
            Assert.Empty(context.Appointments);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_If_NotOwner()
        {
            var context = GetInMemoryContext();
            var service = new AppointmentService(context);

            context.Appointments.Add(new Appointment
            {
                Id = 1,
                UserId = "user1",
                Date = DateOnly.FromDateTime(DateTime.Today),
                TimeSlot = TimeSpan.FromHours(9)
            });
            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(1, "otherUser");

            Assert.False(result);
        }

        [Fact]
        public async Task GetAppointmentsByUserIdAsync_Should_Return_Only_User_Appointments()
        {
            var context = GetInMemoryContext();
            var service = new AppointmentService(context);

            context.Barbers.Add(new TheCutHub.Models.Barber { Id = 1, UserId = "barber1", FullName = "B1" });
            context.Services.Add(new Service { Id = 1, Name = "Test Cut", DurationMinutes = 30 });
            await context.SaveChangesAsync();

            var today = DateOnly.FromDateTime(DateTime.Today);

            var appointment1 = new Appointment
            {
                Id = 1,
                UserId = "user1",
                BarberId = 1,
                ServiceId = 1,
                Date = today,
                TimeSlot = TimeSpan.FromHours(9)
            };
            var appointment2 = new Appointment
            {
                Id = 2,
                UserId = "user2",
                BarberId = 1,
                ServiceId = 1,
                Date = today,
                TimeSlot = TimeSpan.FromHours(10)
            };
            await context.Appointments.AddRangeAsync(appointment1, appointment2);
            await context.SaveChangesAsync();

            Assert.Equal(2, await context.Appointments.CountAsync());

            var result = await service.GetAppointmentsByUserIdAsync("user1");

            Assert.Single(result);
            Assert.Equal("user1", result.First().UserId);
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_Should_Respect_WorkingHours_And_Appointments()
        {
            var context = GetInMemoryContext();
            var service = new AppointmentService(context);

            var today = DateOnly.FromDateTime(DateTime.Today);

            await context.WorkingHours.AddAsync(new WorkingHour
            {
                Id = 1,
                BarberId = 1,
                Day = today.DayOfWeek,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(10, 0, 0),
                SlotIntervalInMinutes = 30,
                IsWorking = true
            });

            await context.Services.AddAsync(new Service
            {
                Id = 1,
                Name = "Test",
                DurationMinutes = 30
            });

            await context.Appointments.AddAsync(new Appointment
            {
                BarberId = 1,
                ServiceId = 1,
                UserId = "u1",
                Date = today,
                TimeSlot = new TimeSpan(9, 0, 0)
            });

            await context.SaveChangesAsync();

            var slots = await service.GetAvailableSlotsAsync(today, TimeSpan.FromMinutes(30), 1);

            Assert.Single(slots);
            Assert.Equal(new TimeSpan(9, 30, 0), slots.First());
        }

        [Fact]
        public async Task GetBarbers_Should_Return_All_Barbers()
        {
            var ctx = GetInMemoryContext();
            var svc = new AppointmentService(ctx);

            ctx.Barbers.AddRange(
                new TheCutHub.Models.Barber { Id = 1, UserId = "barber1", FullName = "A" },
                new TheCutHub.Models.Barber { Id = 2, UserId = "barber1", FullName = "B" }
            );
            await ctx.SaveChangesAsync();

            var barbers = svc.GetBarbers().ToList();

            Assert.Equal(2, barbers.Count);
            Assert.Contains(barbers, b => b.FullName == "A");
        }

        [Fact]
        public async Task GetServices_Should_Return_All_Services()
        {
            var ctx = GetInMemoryContext();
            var svc = new AppointmentService(ctx);

            ctx.Services.AddRange(
                new Service { Id = 1, Name = "S1" },
                new Service { Id = 2, Name = "S2" }
            );
            await ctx.SaveChangesAsync();

            var services = svc.GetServices().ToList();

            Assert.Equal(2, services.Count);
        }

        [Fact]
        public async Task GetServiceByIdAsync_Should_Return_Service_Or_Null()
        {
            var ctx = GetInMemoryContext();
            var svc = new AppointmentService(ctx);

            ctx.Services.Add(new Service { Id = 5, Name = "Test" });
            await ctx.SaveChangesAsync();

            var existing = await svc.GetServiceByIdAsync(5);
            var missing = await svc.GetServiceByIdAsync(999);

            Assert.NotNull(existing);
            Assert.Equal("Test", existing.Name);
            Assert.Null(missing);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Include_Navigation_Properties()
        {
            var ctx = GetInMemoryContext();
            var svc = new AppointmentService(ctx);

            var user = new ApplicationUser { Id = "u1", UserName = "u1" };
            var barber = new TheCutHub.Models.Barber { Id = 1, FullName = "B1", UserId = "barber1" };
            var service = new Service { Id = 1, Name = "Cut", DurationMinutes = 15 };

            var appt = new Appointment
            {
                Id = 10,
                UserId = user.Id,
                BarberId = barber.Id,
                ServiceId = service.Id,
                Date = DateOnly.FromDateTime(DateTime.Today),
                TimeSlot = TimeSpan.Zero,
                User = user,
                Barber = barber,
                Service = service
            };

            ctx.Users.Add(user);
            ctx.Barbers.Add(barber);
            ctx.Services.Add(service);
            ctx.Appointments.Add(appt);
            await ctx.SaveChangesAsync();

            var result = await svc.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal("u1", result?.User?.UserName);
            Assert.Equal("B1", result?.Barber?.FullName);
            Assert.Equal("Cut", result?.Service?.Name);
        }

        [Fact]
        public async Task EditAsync_Should_Update_Existing_Appointment()
        {
            var ctx = GetInMemoryContext();
           var svc = new AppointmentService(ctx);
        
          var appt = new Appointment { Id = 1, UserId = "u1", Date = DateOnly.FromDateTime(DateTime.Today) };
            ctx.Appointments.Add(appt);
           await ctx.SaveChangesAsync();
        
            appt.Date = appt.Date.AddDays(1);
            await svc.EditAsync(appt);
        
            var updated = await ctx.Appointments.FindAsync(1);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Today).AddDays(1), updated?.Date);
        }
    }
}
