using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using TheCutHub.Areas.Barber.Services;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;
using Xunit;

namespace TheCutHub.Tests.Services.Barber
{
    public class BarberProfileServiceTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private IWebHostEnvironment GetMockEnv(string webRootPath = "/fake-root")
        {
            var mock = new Mock<IWebHostEnvironment>();
            mock.Setup(e => e.WebRootPath).Returns(webRootPath);
            return mock.Object;
        }

        [Fact]
        public async Task GetProfileAsync_Should_Return_ViewModel()
        {
            var context = GetInMemoryContext();
            var env = GetMockEnv();
            var service = new BarberProfileService(context, env);

            var barber = new TheCutHub.Models.Barber
            {
                Id = 1,
                UserId = "b1",
                FullName = "Barber Name",
                Bio = "Test bio",
                ProfileImageUrl = "/img.jpg",
                WorkImages = new System.Collections.Generic.List<WorkImage>
                {
                    new WorkImage { Id = 1, ImageUrl = "/img1.jpg" }
                }
            };

            context.Barbers.Add(barber);
            await context.SaveChangesAsync();

            var result = await service.GetProfileAsync("b1");

            Assert.NotNull(result);
            Assert.Equal("Barber Name", result.FullName);
            Assert.Single(result.WorkImages);
        }

        [Fact]
        public async Task UpdateProfileAsync_Should_Update_Barber_Data()
        {
            var context = GetInMemoryContext();
            var env = GetMockEnv();
            var service = new BarberProfileService(context, env);

            var barber = new TheCutHub.Models.Barber { Id = 1, UserId = "b1", FullName = "Old Name", Bio = "Old Bio" };
            context.Barbers.Add(barber);
            await context.SaveChangesAsync();

            var model = new BarberProfileEditViewModel { FullName = "New Name", Bio = "New Bio" };
            var result = await service.UpdateProfileAsync("b1", model);

            Assert.True(result);
            var updated = await context.Barbers.FirstAsync();
            Assert.Equal("New Name", updated.FullName);
            Assert.Equal("New Bio", updated.Bio);
        }

        [Fact]
        public async Task AddWorkImageAsync_Should_Add_Image_When_Valid()
        {
            var context = GetInMemoryContext();
            var env = GetMockEnv();
            var service = new BarberProfileService(context, env);

            var barber = new TheCutHub.Models.Barber { Id = 1, UserId = "b1" };
            context.Barbers.Add(barber);
            await context.SaveChangesAsync();

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1);
            mockFile.Setup(f => f.FileName).Returns("test.jpg");
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            var model = new AddWorkImageViewModel { ImageFile = mockFile.Object };
            var result = await service.AddWorkImageAsync("b1", model);

            Assert.True(result);
            Assert.Single(context.WorkImages);
        }

        [Fact]
        public async Task DeleteWorkImageAsync_Should_Remove_Image()
        {
            var context = GetInMemoryContext();
            var env = GetMockEnv();
            var service = new BarberProfileService(context, env);

            var barber = new TheCutHub.Models.Barber { Id = 1, UserId = "b1" };
            var image = new WorkImage { Id = 5, BarberId = 1, ImageUrl = "/img.jpg" };

            context.Barbers.Add(barber);
            context.WorkImages.Add(image);
            await context.SaveChangesAsync();

            var result = await service.DeleteWorkImageAsync("b1", 5);

            Assert.True(result);
            Assert.Empty(context.WorkImages);
        }

        [Fact]
        public async Task DeleteWorkImageAsync_Should_Return_False_If_Not_Found()
        {
            var context = GetInMemoryContext();
            var env = GetMockEnv();
            var service = new BarberProfileService(context, env);

            var barber = new TheCutHub.Models.Barber { Id = 1, UserId = "b1" };
            context.Barbers.Add(barber);
            await context.SaveChangesAsync();

            var result = await service.DeleteWorkImageAsync("b1", 999);

            Assert.False(result);
        }
    }
}
