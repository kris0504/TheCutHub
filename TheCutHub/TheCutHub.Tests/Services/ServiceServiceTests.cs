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
        public async Task CreateAsync_AddsServiceToDatabase()
        {
            var context = GetInMemoryDbContext();
            var service = new ServiceService(context);

            var newService = new Service
            {
                Name = "Haircut",
                Description = "Basic haircut",
                Price = 20,
                DurationMinutes = 30
            };

            await service.CreateAsync(newService);

            var result = await context.Services.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("Haircut", result.Name);
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
        public async Task UpdateAsync_UpdatesService()
        {
            var context = GetInMemoryDbContext();
            var s = new Service { Id = 5, Name = "Old", Price = 5 };
            context.Services.Add(s);
            await context.SaveChangesAsync();

            var service = new ServiceService(context);
            s.Name = "Updated";
            s.Price = 50;

            await service.UpdateAsync(s);

            var updated = await context.Services.FindAsync(5);
            Assert.Equal("Updated", updated!.Name);
            Assert.Equal(50, updated.Price);
        }

        [Fact]
        public async Task DeleteAsync_RemovesService_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var s = new Service { Id = 3, Name = "ToDelete" };
            context.Services.Add(s);
            await context.SaveChangesAsync();

            var service = new ServiceService(context);
            var result = await service.DeleteAsync(3);

            Assert.True(result);
            Assert.Empty(context.Services);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            var context = GetInMemoryDbContext();
            var service = new ServiceService(context);

            var result = await service.DeleteAsync(999);

            Assert.False(result);
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