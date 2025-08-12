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
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;

namespace TheCutHub.Tests.Controllers
{
    public class ReviewsControllerTests
    {
		private static Mock<UserManager<ApplicationUser>> MockUserManager()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
		}
		private static ApplicationDbContext GetInMemoryContext()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			return new ApplicationDbContext(options);
		}

		private ReviewsController SetupController(
			IReviewService reviewService,
		UserManager<ApplicationUser> userManager,
		ClaimsPrincipal? user = null)
		{
			var ctx = GetInMemoryContext();
			var controller = new ReviewsController(reviewService, userManager, ctx);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = user ?? new ClaimsPrincipal(new ClaimsIdentity())
				}
			};
			controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
			return controller;
		}

        [Fact]
        public async Task Add_Should_Redirect_With_Error_When_Model_Invalid()
        {
            var reviewServiceMock = new Mock<IReviewService>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

			var controller = SetupController(reviewServiceMock.Object, userManagerMock.Object);
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

			var controller = SetupController(reviewServiceMock.Object, userManagerMock.Object);


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
			reviewServiceMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Review)null!);

			var userManagerMock = MockUserManager();
			var controller = SetupController(reviewServiceMock.Object, userManagerMock.Object);


			var result = await controller.Delete(1);
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Delete_Should_Return_Forbid_If_Not_Author_And_Not_Admin()
		{
			var review = new Review
			{
				Id = 1,
				BarberId = 42,
				UserId = "author-xyz",
				Barber = new Barber() 
			};

			var reviewSvc = new Mock<IReviewService>();
			reviewSvc.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(review);

			var um = MockUserManager();
			
			um.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("some-other-user");

			var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] {
			new Claim(ClaimTypes.Name, "unauthorizedUser") 
        }, "TestAuth"));

			var ctrl = SetupController(reviewSvc.Object, um.Object, claims);

			var result = await ctrl.Delete(1);
			Assert.IsType<ForbidResult>(result);
		}

		[Fact]
		public async Task Delete_Should_Delete_If_Barber_Owner()
		{
			var review = new Review
			{
				Id = 1,
				BarberId = 42,
				Barber = new Barber { User = new ApplicationUser { UserName = "barberUser" } }
			};

			var reviewServiceMock = new Mock<IReviewService>();
			reviewServiceMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
			reviewServiceMock.Setup(r => r.DeleteAsync(review)).Returns(Task.CompletedTask).Verifiable();

			var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
			{
		new Claim(ClaimTypes.Name, "barberUser")
	}, "TestAuth"));

			var userManagerMock = MockUserManager();
			var controller = SetupController(reviewServiceMock.Object, userManagerMock.Object, claims);
			var result = await controller.Delete(1);

			reviewServiceMock.Verify(r => r.DeleteAsync(review), Times.Once);

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Details", redirect.ActionName);
			Assert.Equal("Barbers", redirect.ControllerName);
		}

	}
}
