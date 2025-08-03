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
    public class AdminServiceServiceTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Services()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminServiceService(ctx);

            await ctx.Services.AddRangeAsync(
                new Service { Id = 1, Name = "Haircut", DurationMinutes = 30 },
                new Service { Id = 2, Name = "Shave", DurationMinutes = 15 }
            );
            await ctx.SaveChangesAsync();

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CreateAsync_Should_Add_Service()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminServiceService(ctx);

            var newService = new Service { Name = "Beard Trim", DurationMinutes = 20 };
            await service.CreateAsync(newService);

            Assert.Single(ctx.Services);
            Assert.Equal("Beard Trim", ctx.Services.First().Name);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Service()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminServiceService(ctx);

            var s = new Service { Id = 5, Name = "Test Service", DurationMinutes = 40 };
            await ctx.Services.AddAsync(s);
            await ctx.SaveChangesAsync();

            var result = await service.GetByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal("Test Service", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Service()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminServiceService(ctx);

            var s = new Service { Id = 10, Name = "Old Name", DurationMinutes = 25 };
            ctx.Services.Add(s);
            await ctx.SaveChangesAsync();

            s.Name = "New Name";
            s.DurationMinutes = 45;
            await service.UpdateAsync(s);

            var updated = await ctx.Services.FindAsync(10);
            Assert.Equal("New Name", updated.Name);
            Assert.Equal(45, updated.DurationMinutes);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Service()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminServiceService(ctx);

            var s = new Service { Id = 100, Name = "To Delete", DurationMinutes = 10 };
            ctx.Services.Add(s);
            await ctx.SaveChangesAsync();

            var result = await service.DeleteAsync(100);

            Assert.True(result);
            Assert.Empty(ctx.Services);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_If_Not_Found()
        {
            var ctx = GetInMemoryContext();
            var service = new AdminServiceService(ctx);

            var result = await service.DeleteAsync(999);

            Assert.False(result);
        }
    }
}
