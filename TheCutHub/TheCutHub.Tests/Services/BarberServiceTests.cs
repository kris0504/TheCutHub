using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using Xunit;
namespace TheCutHub.Tests.Services
{
    public class BarberServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BarberTestDb_" + Guid.NewGuid())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_AddsBarberToDatabase()
        {

            var context = GetInMemoryDbContext();
            var service = new BarberService(context);
            var barber = new TheCutHub.Models.Barber { FullName = "Test Barber", Bio = "Bio", UserId = "u1" };



            await service.CreateAsync(barber);


            Assert.Equal(1, await context.Barbers.CountAsync());
            Assert.Equal("Test Barber", (await context.Barbers.FirstAsync()).FullName);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllBarbers()
        {
            var context = GetInMemoryDbContext();
            context.Barbers.AddRange(
                new TheCutHub.Models.Barber { FullName = "B1", UserId = "u2" },
                new TheCutHub.Models.Barber { FullName = "B2", UserId = "u3" });
            await context.SaveChangesAsync();

            var service = new BarberService(context);

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectBarber()
        {
            var context = GetInMemoryDbContext();
            context.Barbers.Add(new TheCutHub.Models.Barber { Id = 5, FullName = "Barber 5", UserId = "u4" });
            await context.SaveChangesAsync();

            var service = new BarberService(context);
            var barber = await service.GetByIdAsync(5);

            Assert.NotNull(barber);
            Assert.Equal("Barber 5", barber.FullName);
        }

        [Fact]
        public async Task DeleteAsync_RemovesBarber()
        {
            var context = GetInMemoryDbContext();
            context.Barbers.Add(new TheCutHub.Models.Barber { Id = 10, FullName = "To Delete", UserId = "u5" });
            await context.SaveChangesAsync();

            var service = new BarberService(context);
            var result = await service.DeleteAsync(10);

            Assert.True(result);
            Assert.Empty(context.Barbers);
        }

        [Fact]
        public void Exists_ReturnsCorrectBool()
        {
            var context = GetInMemoryDbContext();
            context.Barbers.Add(new TheCutHub.Models.Barber { Id = 2, UserId = "u6" });
            context.SaveChanges();

            var service = new BarberService(context);

            Assert.True(service.Exists(2));
            Assert.False(service.Exists(99));
        }
    }
}