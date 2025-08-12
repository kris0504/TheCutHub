using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheCutHub.Areas.Barber.Controllers;
using TheCutHub.Areas.Barber.Services;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;
using Xunit;

namespace TheCutHub.Tests.Controllers.Barbers
{
    public class BarberProfileControllerTests
    {
        private readonly Mock<IBarberProfileService> _mockProfileService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ProfileController _controller;
        private readonly ApplicationUser _mockUser;

		public BarberProfileControllerTests()
		{
			_mockProfileService = new Mock<IBarberProfileService>();

			var store = new Mock<IUserStore<ApplicationUser>>();
			_mockUserManager = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			_mockUser = new ApplicationUser { Id = "barber123", UserName = "barber" };
			_mockUserManager
				.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
				.Returns("barber123");
			_mockUserManager
				.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
				.ReturnsAsync(_mockUser);
			
			var http = new DefaultHttpContext();

			_controller = new ProfileController(_mockProfileService.Object, _mockUserManager.Object)
			{
				ControllerContext = new ControllerContext { HttpContext = http },
				TempData = new TempDataDictionary(http, Mock.Of<ITempDataProvider>())
			};
		}

		[Fact]
		public async Task Edit_Get_ReturnsView_WhenProfileExists()
		{
			var vm = new BarberProfileEditViewModel();
			_mockProfileService
				.Setup(s => s.GetProfileAsync("barber123"))
				.ReturnsAsync(vm);

			
			var claims = new[]
			{
		new Claim(ClaimTypes.NameIdentifier, "barber123"),
		new Claim(ClaimTypes.Name, "barber"),
		new Claim(ClaimTypes.Role, "Barber"),
	};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var user = new ClaimsPrincipal(identity);

			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext { User = user }
			};
			_controller.TempData = new TempDataDictionary(
				_controller.ControllerContext.HttpContext,
				Mock.Of<ITempDataProvider>());

			
			var result = await _controller.Edit();

			
			var view = Assert.IsType<ViewResult>(result);
			Assert.Equal(vm, view.Model);
		}

		[Fact]
        public async Task Edit_Get_ReturnsNotFound_WhenProfileIsNull()
        {
            _mockProfileService.Setup(s => s.GetProfileAsync(_mockUser.Id)).ReturnsAsync((BarberProfileEditViewModel)null!);

            var result = await _controller.Edit();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithImages()
        {
            var vm = new BarberProfileEditViewModel();
            _controller.ModelState.AddModelError("FullName", "Required");

            _mockProfileService.Setup(s => s.GetProfileAsync(_mockUser.Id)).ReturnsAsync(new BarberProfileEditViewModel
            {
                WorkImages = [new WorkImage { ImageUrl = "/image1.jpg" }]
            });

            var result = await _controller.Edit(vm);

            var view = Assert.IsType<ViewResult>(result);
            Assert.NotNull(((BarberProfileEditViewModel)view.Model).WorkImages);
        }

        [Fact]
        public async Task Edit_Post_ProfileNotFound_RedirectsToEditWithFail()
        {
            var vm = new BarberProfileEditViewModel();

            _mockProfileService.Setup(s => s.UpdateProfileAsync(_mockUser.Id, vm)).ReturnsAsync(false);

            var result = await _controller.Edit(vm);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirect.ActionName);
            Assert.Equal("Profile not found.", _controller.TempData["Fail"]);
        }

        [Fact]
        public async Task Edit_Post_SuccessfulUpdate_RedirectsToEditWithSuccess()
        {
            var vm = new BarberProfileEditViewModel();

            _mockProfileService.Setup(s => s.UpdateProfileAsync(_mockUser.Id, vm)).ReturnsAsync(true);

            var result = await _controller.Edit(vm);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirect.ActionName);
            Assert.Equal("Profile successfully updated.", _controller.TempData["Success"]);
        }

        [Fact]
        public async Task AddWorkImage_InvalidModel_RedirectsWithError()
        {
            _controller.ModelState.AddModelError("ImageFile", "Required");
            var result = await _controller.AddWorkImage(new AddWorkImageViewModel());

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirect.ActionName);
            Assert.Equal("Form contains invalid data.", _controller.TempData["Error"]);
        }

        [Fact]
        public async Task AddWorkImage_UploadFails_RedirectsWithFail()
        {
            var vm = new AddWorkImageViewModel();
            _mockProfileService.Setup(s => s.AddWorkImageAsync(_mockUser.Id, vm)).ReturnsAsync(false);

            var result = await _controller.AddWorkImage(vm);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirect.ActionName);
            Assert.Equal("Only JPG, PNG and WebP files are allowed.", _controller.TempData["Fail"]);
        }

        [Fact]
        public async Task AddWorkImage_Success_RedirectsWithSuccess()
        {
            var vm = new AddWorkImageViewModel();
            _mockProfileService.Setup(s => s.AddWorkImageAsync(_mockUser.Id, vm)).ReturnsAsync(true);

            var result = await _controller.AddWorkImage(vm);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirect.ActionName);
            Assert.Equal("Image was successfully added!", _controller.TempData["Success"]);
        }

        [Fact]
        public async Task DeleteWorkImage_Success_DeletesAndRedirectsWithSuccess()
        {
            _mockProfileService.Setup(s => s.DeleteWorkImageAsync(_mockUser.Id, 10)).ReturnsAsync(true);

            var result = await _controller.DeleteWorkImage(10);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirect.ActionName);
            Assert.Equal("Image successfully deleted.", _controller.TempData["Success"]);
        }

        [Fact]
        public async Task DeleteWorkImage_NotFound_RedirectsWithFail()
        {
            _mockProfileService.Setup(s => s.DeleteWorkImageAsync(_mockUser.Id, 999)).ReturnsAsync(false);

            var result = await _controller.DeleteWorkImage(999);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirect.ActionName);
            Assert.Equal("Image not found.", _controller.TempData["Fail"]);
        }
    }
}
