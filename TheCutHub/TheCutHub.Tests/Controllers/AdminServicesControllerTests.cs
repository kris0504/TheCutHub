using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheCutHub.Areas.Admin.Controllers;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Models;
using Xunit;

namespace TheCutHub.Tests.Controllers.Admin
{
    public class AdminServicesControllerTests
    {
        private readonly Mock<IAdminServiceService> _mockService;
        private readonly ServicesController _controller;

        public AdminServicesControllerTests()
        {
            _mockService = new Mock<IAdminServiceService>();
            _controller = new ServicesController(_mockService.Object);
        }

        private Service GetFakeService(int id = 1) => new Service { Id = id, Name = "Shave", Price = 10 };
		[Fact]
		public async Task Index_ReturnsView_WithPagedModel()
		{
			var services = new List<Service> { GetFakeService(), GetFakeService(2) };
			var paged = X.PagedList.PagedListExtensions.ToPagedList(services, 1, 5);

			_mockService
				.Setup(s => s.GetPagedAsync(null, 1, 5))
				.ReturnsAsync(paged);

			var result = await _controller.Index(null);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Same(paged, viewResult.Model);
			_mockService.Verify(s => s.GetPagedAsync(null, 1, 5), Times.Once);
		}



		[Fact]
        public void Create_Get_ReturnsView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            
            var service = GetFakeService();
            _controller.ModelState.AddModelError("Name", "Required");

            
            var result = await _controller.Create(service);

            
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(service, viewResult.Model);
        }
        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            
            var service = GetFakeService();

            
            var result = await _controller.Create(service);

            
            _mockService.Verify(s => s.CreateAsync(service), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Fact]
        public async Task Edit_Get_ExistingId_ReturnsViewWithModel()
        {
            var service = GetFakeService();
            _mockService.Setup(s => s.GetByIdAsync(service.Id)).ReturnsAsync(service);

            var result = await _controller.Edit(service.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(service, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Service)null!);

            var result = await _controller.Edit(99);

            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task Edit_Post_IdMismatch_ReturnsNotFound()
        {
            var service = GetFakeService(1);

            var result = await _controller.Edit(2, service);

            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsView()
        {
            var service = GetFakeService();
            _controller.ModelState.AddModelError("Price", "Required");

            var result = await _controller.Edit(service.Id, service);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(service, viewResult.Model);
        }
        [Fact]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            var service = GetFakeService();

            var result = await _controller.Edit(service.Id, service);

            _mockService.Verify(s => s.UpdateAsync(service), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
        [Fact]
        public async Task Delete_Get_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Service)null!);

            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task Delete_Get_ValidId_ReturnsViewWithModel()
        {
            var service = GetFakeService();
            _mockService.Setup(s => s.GetByIdAsync(service.Id)).ReturnsAsync(service);

            var result = await _controller.Delete(service.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(service, viewResult.Model);
        }
        [Fact]
        public async Task DeleteConfirmed_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _controller.DeleteConfirmed(99);

            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteConfirmed(1);

            _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

    }
}
