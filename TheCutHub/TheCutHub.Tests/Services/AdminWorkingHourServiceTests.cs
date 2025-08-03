using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Data;
using TheCutHub.Models;
using Xunit;

namespace TheCutHub.Tests.Services.Admin
{
    public class AdminWorkingHourServiceTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllBarbersAsync_Should_Return_Barbers()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminWorkingHourService(ctx);

            await ctx.Barbers.AddRangeAsync(
                new TheCutHub.Models.Barber { Id = 1, UserId = "u1", FullName = "Barber 1" },
                new TheCutHub.Models.Barber { Id = 2, UserId = "u2", FullName = "Barber 2" }
            );
            await ctx.SaveChangesAsync();

            var result = await service.GetAllBarbersAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_WorkingHour_With_Barber()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminWorkingHourService(ctx);

            var barber = new TheCutHub.Models.Barber { Id = 1, UserId = "u1", FullName = "Barber 1" };
            var wh = new WorkingHour { Id = 10, BarberId = 1, Barber = barber, Day = DayOfWeek.Monday };

            await ctx.Barbers.AddAsync(barber);
            await ctx.WorkingHours.AddAsync(wh);
            await ctx.SaveChangesAsync();

            var result = await service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal("Barber 1", result.Barber.FullName);
        }

        [Fact]
        public async Task GetByBarberIdAsync_Should_Return_All_For_Barber()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminWorkingHourService(ctx);

            await ctx.WorkingHours.AddRangeAsync(
                new WorkingHour { Id = 1, BarberId = 1, Day = DayOfWeek.Monday },
                new WorkingHour { Id = 2, BarberId = 1, Day = DayOfWeek.Tuesday },
                new WorkingHour { Id = 3, BarberId = 2, Day = DayOfWeek.Monday }
            );
            await ctx.SaveChangesAsync();

            var result = await service.GetByBarberIdAsync(1);

            Assert.Equal(2, result.Count);
            Assert.All(result, w => Assert.Equal(1, w.BarberId));
        }

        [Fact]
        public async Task CreateAsync_Should_Add_WorkingHour()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminWorkingHourService(ctx);

            var wh = new WorkingHour
            {
                BarberId = 1,
                Day = DayOfWeek.Monday,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17),
                IsWorking = true,
                SlotIntervalInMinutes = 30
            };

            await service.CreateAsync(wh);

            Assert.Single(ctx.WorkingHours);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_WorkingHour()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminWorkingHourService(ctx);

            var wh = new WorkingHour { Id = 1, BarberId = 1, Day = DayOfWeek.Monday, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(17), IsWorking = true, SlotIntervalInMinutes = 30 };
            await ctx.WorkingHours.AddAsync(wh);
            await ctx.SaveChangesAsync();

            wh.StartTime = TimeSpan.FromHours(10);
            wh.EndTime = TimeSpan.FromHours(18);
            wh.IsWorking = false;
            wh.SlotIntervalInMinutes = 60;

            var result = await service.UpdateAsync(wh);

            Assert.True(result);
            var updated = await ctx.WorkingHours.FindAsync(1);
            Assert.Equal(TimeSpan.FromHours(10), updated.StartTime);
            Assert.Equal(TimeSpan.FromHours(18), updated.EndTime);
            Assert.False(updated.IsWorking);
            Assert.Equal(60, updated.SlotIntervalInMinutes);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Existing_WorkingHour()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminWorkingHourService(ctx);

            var wh = new WorkingHour { Id = 5, BarberId = 1 };
            await ctx.WorkingHours.AddAsync(wh);
            await ctx.SaveChangesAsync();

            var result = await service.DeleteAsync(5);

            Assert.True(result);
            Assert.Empty(ctx.WorkingHours);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_If_Not_Found()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminWorkingHourService(ctx);

            var result = await service.DeleteAsync(999);

            Assert.False(result);
        }
    }
}
