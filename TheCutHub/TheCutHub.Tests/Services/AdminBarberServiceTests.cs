using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Areas.Admin.Services;
using Xunit;

namespace TheCutHub.Tests.Services.Admin
{
    public class AdminBarberServiceTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Barbers()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminBarberService(ctx);

            await ctx.Barbers.AddRangeAsync(
                new TheCutHub.Models.Barber { Id = 1, UserId = "b1", FullName = "Barber 1" },
                new TheCutHub.Models.Barber { Id = 2, UserId = "b2", FullName = "Barber 2" }
            );
            await ctx.SaveChangesAsync();

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CreateAsync_Should_Add_Barber()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminBarberService(ctx);

            var barber = new TheCutHub.Models.Barber { UserId = "b3", FullName = "Barber 3" };
            await service.CreateAsync(barber);

            Assert.Single(ctx.Barbers);
            Assert.Equal("Barber 3", ctx.Barbers.First().FullName);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Barber()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminBarberService(ctx);

            var barber = new TheCutHub.Models.Barber { Id = 5, UserId = "b5", FullName = "Barber 5" };
            await ctx.Barbers.AddAsync(barber);
            await ctx.SaveChangesAsync();

            var result = await service.GetByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal("Barber 5", result.FullName);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Barber()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminBarberService(ctx);

            var barber = new TheCutHub.Models.Barber { Id = 10, UserId = "b10", FullName = "Old Name" };
            ctx.Barbers.Add(barber);
            await ctx.SaveChangesAsync();

            barber.FullName = "Updated Name";
            await service.UpdateAsync(barber);

            var updated = await ctx.Barbers.FindAsync(10);
            Assert.Equal("Updated Name", updated.FullName);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Barber()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminBarberService(ctx);

            var barber = new TheCutHub.Models.Barber { Id = 100, UserId = "b100", FullName = "To Remove" };
            ctx.Barbers.Add(barber);
            await ctx.SaveChangesAsync();

            var success = await service.DeleteAsync(100);
            Assert.True(success);
            Assert.Empty(ctx.Barbers);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_If_Not_Found()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminBarberService(ctx);

            var result = await service.DeleteAsync(999);
            Assert.False(result);
        }
    }
}
