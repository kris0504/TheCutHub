using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheCutHub.Areas.Admin.Controllers;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Models;
using Xunit;


namespace TheCutHub.Tests.Controllers.Admin
{
    public class AdminUsersControllerTests
    {
        private readonly Mock<IAdminUserService> _mockUserService;
        private readonly UsersController _controller;

        public AdminUsersControllerTests()
        {
            _mockUserService = new Mock<IAdminUserService>();
            _controller = new UsersController(_mockUserService.Object);
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        private ApplicationUser GetFakeUser(string id = "user1") => new ApplicationUser { Id = id, UserName = "john" };
        [Fact]
        public async Task Index_ReturnsView_WithUsersAndRoles()
        {
            var users = new List<ApplicationUser> { GetFakeUser(), GetFakeUser("user2") };
            var roles = new Dictionary<string, string> { { "user1", "User" }, { "user2", "Barber" } };

            _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);
            _mockUserService.Setup(s => s.GetUserRolesMapAsync())
                    .ReturnsAsync(new Dictionary<string, IList<string>>
                    {
                        { "user1", new List<string> { "User" } },
                        { "user2", new List<string> { "Barber" } }
                    });

            var result = await _controller.Index(null,null,null);

            
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(users, viewResult.Model);
            Assert.NotNull(_controller.ViewBag.UserRoles);


        }
        [Fact]
        public async Task MakeBarber_ValidUserId_RedirectsToIndex_AndSetsTempData()
        {
            var userId = "user1";
            _mockUserService.Setup(s => s.MakeBarberAsync(userId)).ReturnsAsync(true);

            var result = await _controller.MakeBarber(userId);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("User is now a barber.", _controller.TempData["BarberCreated"]);
        }
        [Fact]
        public async Task MakeBarber_InvalidUserId_ReturnsNotFound()
        {
            var userId = "invalid";
            _mockUserService.Setup(s => s.MakeBarberAsync(userId)).ReturnsAsync(false);

            var result = await _controller.MakeBarber(userId);

            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task RemoveBarber_ValidUserId_RedirectsToIndex_AndSetsTempData()
        {
            var userId = "user1";
            _mockUserService.Setup(s => s.RemoveBarberAsync(userId)).ReturnsAsync(true);

            var result = await _controller.RemoveBarber(userId);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("User is no longer a barber.", _controller.TempData["BarberRemoved"]);
        }
        [Fact]
        public async Task RemoveBarber_InvalidUserId_ReturnsNotFound()
        {
            var userId = "invalid";
            _mockUserService.Setup(s => s.RemoveBarberAsync(userId)).ReturnsAsync(false);

            var result = await _controller.RemoveBarber(userId);

            Assert.IsType<NotFoundResult>(result);
        }


    }

}
