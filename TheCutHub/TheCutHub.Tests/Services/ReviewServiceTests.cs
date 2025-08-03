using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Services;
using Xunit;
namespace TheCutHub.Tests.Services
{
    public class ReviewServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ReviewServiceDb_" + Guid.NewGuid())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddAsync_AddsReviewToDatabase()
        {

            var context = GetInMemoryDbContext();

            var barber = new TheCutHub.Models.Barber { Id = 1, UserId = "barber-user-id", FullName = "Test Barber" };
            context.Barbers.Add(barber);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);


            await service.AddAsync(barber.Id, "user123", "Great!", 5, DateTime.UtcNow);


            var review = await context.Reviews.FirstOrDefaultAsync();
            Assert.NotNull(review);
            Assert.Equal("Great!", review.Comment);
            Assert.Equal(5, review.Rating);
        }

        [Fact]

        public async Task GetByIdAsync_ReturnsReview()
        {
            var context = GetInMemoryDbContext();

            var user = new ApplicationUser { Id = "userX", UserName = "userX@test.bg", Email = "userX@test.bg" };
            var barberUser = new ApplicationUser { Id = "barber-user", UserName = "barber@test.bg", Email = "barber@test.bg" };
            var barber = new TheCutHub.Models.Barber { Id = 1, UserId = "barber-user", FullName = "Barber", User = barberUser };

            context.Users.Add(user);
            context.Users.Add(barberUser);
            context.Barbers.Add(barber);

            var review = new Review
            {
                Id = 10,
                BarberId = 1,
                UserId = "userX",
                Comment = "Very good!",
                Rating = 4,
                CreatedOn = DateTime.UtcNow
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);
            var result = await service.GetByIdAsync(10);

            Assert.NotNull(result);
            Assert.Equal("Very good!", result.Comment);
        }

        [Fact]
        public async Task DeleteAsync_RemovesReview()
        {
            var context = GetInMemoryDbContext();

            var review = new Review { Id = 3, BarberId = 1, UserId = "uid", Comment = "To delete", Rating = 2 };
            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var service = new ReviewService(context);

            await service.DeleteAsync(review);

            Assert.Null(await context.Reviews.FirstOrDefaultAsync(r => r.Id == 3));
        }
    }
}