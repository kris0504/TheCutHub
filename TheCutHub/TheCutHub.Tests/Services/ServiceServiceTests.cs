using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Services;
using Xunit;
namespace TheCutHub.Tests.Services
{
    public class ServiceServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ServiceDb_" + Guid.NewGuid())
                .Options;

            return new ApplicationDbContext(options);
        }

       
        [Fact]
        public async Task GetAllAsync_ReturnsAllServices()
        {
            var context = GetInMemoryDbContext();
            context.Services.AddRange(
                new Service { Name = "Haircut", Price = 20 },
                new Service { Name = "Shave", Price = 15 });
            await context.SaveChangesAsync();

            var service = new ServiceService(context);

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectService()
        {
            var context = GetInMemoryDbContext();
            var s = new Service { Id = 7, Name = "Beard Trim", Price = 10 };
            context.Services.Add(s);
            await context.SaveChangesAsync();

            var service = new ServiceService(context);
            var result = await service.GetByIdAsync(7);

            Assert.NotNull(result);
            Assert.Equal("Beard Trim", result.Name);
        }

       

        [Fact]
        public void Exists_ReturnsTrue_WhenFound()
        {
            var context = GetInMemoryDbContext();
            context.Services.Add(new Service
            {
                Id = 2,
                Name = "TestService",
                Price = 10,
                DurationMinutes = 30
            });
            context.SaveChanges();

            var service = new ServiceService(context);

            Assert.True(service.Exists(2));
        }


        [Fact]
        public void Exists_ReturnsFalse_WhenNotFound()
        {
            var context = GetInMemoryDbContext();
            var service = new ServiceService(context);

            Assert.False(service.Exists(555));
        }
    }
}