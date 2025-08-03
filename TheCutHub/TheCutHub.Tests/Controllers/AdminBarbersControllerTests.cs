using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TheCutHub.Areas.Admin.Controllers;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Models;

namespace TheCutHub.Tests.Controllers.Admin
{
    public class AdminBarbersControllerTests
    {
        private BarbersController GetController(out Mock<IAdminBarberService> mockService)
        {
            mockService = new Mock<IAdminBarberService>();
            return new BarbersController(mockService.Object);
        }

        [Fact]
        public async Task Index_Should_Return_View_With_Barbers()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Models.Barber>());

            var result = await controller.Index();
            var view = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Models.Barber>>(view.Model);
        }

        [Fact]
        public void Create_Get_Should_Return_View()
        {
            var controller = GetController(out _);
            var result = controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_Should_Return_View_When_Model_Invalid()
        {
            var controller = GetController(out _);
            controller.ModelState.AddModelError("Error", "Invalid");

            var model = new Models.Barber();
            var result = await controller.Create(model);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, view.Model);
        }

        [Fact]
        public async Task Create_Post_Should_Redirect_When_Model_Valid()
        {
            var controller = GetController(out var mockService);
            var model = new Models.Barber { Id = 1, FullName = "Test" };

            var result = await controller.Create(model);

            mockService.Verify(s => s.CreateAsync(model), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Get_Should_Return_NotFound_When_Barber_Not_Found()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Barber)null);

            var result = await controller.Edit(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_Should_Return_View_When_Barber_Found()
        {
            var controller = GetController(out var mockService);
            var barber = new Models.Barber { Id = 1 };
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(barber);

            var result = await controller.Edit(1);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(barber, view.Model);
        }

        [Fact]
        public async Task Edit_Post_Should_Return_NotFound_When_Id_Mismatch()
        {
            var controller = GetController(out _);
            var model = new Models.Barber { Id = 2 };

            var result = await controller.Edit(1, model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_Should_Return_View_When_Model_Invalid()
        {
            var controller = GetController(out _);
            controller.ModelState.AddModelError("Error", "Invalid");

            var model = new Models.Barber { Id = 1 };
            var result = await controller.Edit(1, model);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, view.Model);
        }

        [Fact]
        public async Task Edit_Post_Should_Redirect_When_Model_Valid()
        {
            var controller = GetController(out var mockService);
            var model = new Models.Barber { Id = 1 };

            var result = await controller.Edit(1, model);

            mockService.Verify(s => s.UpdateAsync(model), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Delete_Get_Should_Return_NotFound_When_Barber_Null()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Barber)null);

            var result = await controller.Delete(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_Should_Return_View_When_Found()
        {
            var controller = GetController(out var mockService);
            var barber = new Models.Barber { Id = 1 };
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(barber);

            var result = await controller.Delete(1);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(barber, view.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_Should_Return_NotFound_When_Delete_Fails()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            var result = await controller.DeleteConfirmed(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_Should_Redirect_When_Delete_Succeeds()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await controller.DeleteConfirmed(1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }

}
