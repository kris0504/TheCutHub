using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TheCutHub.Controllers;
using TheCutHub.Models;
using TheCutHub.Services;
using Xunit;

namespace TheCutHub.Tests.Controllers
{
    public class BarbersControllerTests
    {
        [Fact]
        public async Task Index_Should_Return_All_Barbers()
        {
            var mockService = new Mock<IBarberService>();
            mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Barber> { new Barber { Id = 1, FullName = "Test" } });

            var controller = new BarbersController(mockService.Object);
            var result = await controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Barber>>(view.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_Should_Return_NotFound_If_Id_Null()
        {
            var controller = new BarbersController(null!);
            var result = await controller.Details(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Should_Return_NotFound_If_NotFound()
        {
            var mockService = new Mock<IBarberService>();
            mockService.Setup(s => s.GetDetailsAsync(1)).ReturnsAsync((Barber)null!);
            var controller = new BarbersController(mockService.Object);

            var result = await controller.Details(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Should_Return_View_If_Found()
        {
            var barber = new Barber { Id = 1, FullName = "Test" };
            var mockService = new Mock<IBarberService>();
            mockService.Setup(s => s.GetDetailsAsync(1)).ReturnsAsync(barber);
            var controller = new BarbersController(mockService.Object);

            var result = await controller.Details(1);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(barber, view.Model);
        }

        [Fact]
        public async Task Create_Post_Should_Redirect_If_Valid()
        {
            var mockService = new Mock<IBarberService>();
            var controller = new BarbersController(mockService.Object);

            var barber = new Barber { Id = 1, FullName = "Test" };
            var result = await controller.Create(barber);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Get_Should_Return_View_If_Found()
        {
            var barber = new Barber { Id = 1 };
            var mockService = new Mock<IBarberService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(barber);

            var controller = new BarbersController(mockService.Object);
            var result = await controller.Edit(1);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(barber, view.Model);
        }

        [Fact]
        public async Task Edit_Post_Should_Return_NotFound_If_Id_Mismatch()
        {
            var controller = new BarbersController(null!);
            var result = await controller.Edit(1, new Barber { Id = 2 });
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_Should_Redirect_If_Valid()
        {
            var mockService = new Mock<IBarberService>();
            mockService.Setup(s => s.Exists(1)).Returns(true);

            var controller = new BarbersController(mockService.Object);
            var result = await controller.Edit(1, new Barber { Id = 1 });

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Delete_Get_Should_Return_View_If_Found()
        {
            var barber = new Barber { Id = 1 };
            var mockService = new Mock<IBarberService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(barber);

            var controller = new BarbersController(mockService.Object);
            var result = await controller.Delete(1);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(barber, view.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_Should_Redirect_If_Success()
        {
            var mockService = new Mock<IBarberService>();
            mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            var controller = new BarbersController(mockService.Object);

            var result = await controller.DeleteConfirmed(1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_Should_Return_NotFound_If_Fail()
        {
            var mockService = new Mock<IBarberService>();
            mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);
            var controller = new BarbersController(mockService.Object);

            var result = await controller.DeleteConfirmed(1);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
