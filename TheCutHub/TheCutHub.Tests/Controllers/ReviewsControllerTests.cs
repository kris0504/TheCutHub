using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TheCutHub.Controllers;
using TheCutHub.Models.ViewModels;
using TheCutHub.Models;
using TheCutHub.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TheCutHub.Tests.Controllers
{
    public class ReviewsControllerTests
    {
        private ReviewsController SetupController(
            Mock<IReviewService> reviewServiceMock,
            Mock<UserManager<ApplicationUser>> userManagerMock,
            ClaimsPrincipal user = null!)
        {
            var controller = new ReviewsController(reviewServiceMock.Object, userManagerMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() }
            };
            return controller;
        }

        [Fact]
        public async Task Add_Should_Redirect_With_Error_When_Model_Invalid()
        {
            var reviewServiceMock = new Mock<IReviewService>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var controller = SetupController(reviewServiceMock, userManagerMock);
            controller.ModelState.AddModelError("Error", "Invalid");

            var model = new AddReviewViewModel { BarberId = 1 };
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            var result = await controller.Add(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Barbers", redirect.ControllerName);
        }

        [Fact]
        public async Task Add_Should_Add_Review_And_Redirect_On_Success()
        {
            var reviewServiceMock = new Mock<IReviewService>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var user = new ApplicationUser { Id = "user123" };
            userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var controller = SetupController(reviewServiceMock, userManagerMock);

            var model = new AddReviewViewModel { BarberId = 1, Comment = "Good", Rating = 5 };
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );
            var result = await controller.Add(model);

            reviewServiceMock.Verify(s =>
                s.AddAsync(1, "user123", "Good", 5, It.IsAny<System.DateTime>()), Times.Once);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Barbers", redirect.ControllerName);
        }

        [Fact]
        public async Task Delete_Should_Return_NotFound_If_Review_Not_Exist()
        {
            var reviewServiceMock = new Mock<IReviewService>();
            reviewServiceMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Review)null);

            var controller = SetupController(reviewServiceMock, Mock.Of<Mock<UserManager<ApplicationUser>>>());

            var result = await controller.Delete(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Should_Return_Forbid_If_Not_Barber_Or_Admin()
        {
            var review = new Review
            {
                Barber = new Barber
                {
                    User = new ApplicationUser { UserName = "otherUser" }
                }
            };

            var reviewServiceMock = new Mock<IReviewService>();
            reviewServiceMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);

            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, "unauthorizedUser")
        }));

            var controller = SetupController(reviewServiceMock, Mock.Of<Mock<UserManager<ApplicationUser>>>(), claims);

            var result = await controller.Delete(1);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Delete_Should_Delete_If_Barber_Owner()
        {
            var review = new Review
            {
                Id = 1,
                BarberId = 42,
                Barber = new Barber
                {
                    User = new ApplicationUser { UserName = "barberUser" }
                }
            };

            var reviewServiceMock = new Mock<IReviewService>();
            reviewServiceMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);

            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, "barberUser")
        }));

            var controller = SetupController(reviewServiceMock, Mock.Of<Mock<UserManager<ApplicationUser>>>(), claims);

            var result = await controller.Delete(1);

            reviewServiceMock.Verify(r => r.DeleteAsync(review), Times.Once);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Barbers", redirect.ControllerName);
        }
    }
}
