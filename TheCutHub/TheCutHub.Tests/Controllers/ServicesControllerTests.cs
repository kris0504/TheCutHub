using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCutHub.Models;
using TheCutHub.Services;
using TheCutHub.Controllers;

namespace TheCutHub.Tests.Controllers
{
    public class ServicesControllerTests
    {
        private ServicesController GetController(out Mock<IServiceService> mockService)
        {
            mockService = new Mock<IServiceService>();
            return new ServicesController(mockService.Object);
        }
        [Fact]
        public async Task Index_Should_Return_View_With_Services()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Service>());

            var result = await controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<Service>>(view.Model);
        }

        [Fact]
        public async Task Details_Should_Return_NotFound_When_Id_Null()
        {
            var controller = GetController(out _);
            var result = await controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Should_Return_NotFound_When_Service_Null()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Service)null);

            var result = await controller.Details(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Should_Return_View_When_Service_Found()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new Service { Id = 1 });

            var result = await controller.Details(1);
            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<Service>(view.Model);
        }

        [Fact]
        public void Create_Get_Should_Return_View()
        {
            var controller = GetController(out _);
            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_Should_Return_View_When_Invalid()
        {
            var controller = GetController(out _);
            controller.ModelState.AddModelError("Error", "Required");

            var model = new Service();
            var result = await controller.Create(model);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, view.Model);
        }

        [Fact]
        public async Task Create_Post_Should_Redirect_When_Valid()
        {
            var controller = GetController(out var mockService);
            var model = new Service { Id = 1, Name = "Haircut" };

            var result = await controller.Create(model);

            mockService.Verify(s => s.CreateAsync(model), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Get_Should_Return_NotFound_When_Id_Null()
        {
            var controller = GetController(out _);
            var result = await controller.Edit(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_Should_Return_NotFound_When_Service_Null()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Service)null);

            var result = await controller.Edit(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_Should_Return_View_When_Found()
        {
            var controller = GetController(out var mockService);
            var model = new Service { Id = 1 };
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(model);

            var result = await controller.Edit(1);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, view.Model);
        }

        [Fact]
        public async Task Edit_Post_Should_Return_NotFound_When_Id_Mismatch()
        {
            var controller = GetController(out _);
            var model = new Service { Id = 2 };

            var result = await controller.Edit(1, model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_Should_Return_View_When_Invalid_Model()
        {
            var controller = GetController(out _);
            controller.ModelState.AddModelError("Error", "Invalid");
            var model = new Service { Id = 1 };

            var result = await controller.Edit(1, model);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, view.Model);
        }

        [Fact]
        public async Task Edit_Post_Should_Redirect_When_Valid()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.Exists(1)).Returns(true);
            var model = new Service { Id = 1 };

            var result = await controller.Edit(1, model);

            mockService.Verify(s => s.UpdateAsync(model), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Post_Should_Return_NotFound_When_Service_Does_Not_Exist()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.Exists(1)).Returns(false);
            mockService.Setup(s => s.UpdateAsync(It.IsAny<Service>())).Throws(new Exception());

            var model = new Service { Id = 1 };

            var result = await controller.Edit(1, model);

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Delete_Get_Should_Return_NotFound_When_Id_Null()
        {
            var controller = GetController(out _);
            var result = await controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_Should_Return_NotFound_When_Service_Null()
        {
            var controller = GetController(out var mockService);
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Service)null);

            var result = await controller.Delete(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_Should_Return_View_When_Service_Found()
        {
            var controller = GetController(out var mockService);
            var service = new Service { Id = 1 };
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(service);

            var result = await controller.Delete(1);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(service, view.Model);
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