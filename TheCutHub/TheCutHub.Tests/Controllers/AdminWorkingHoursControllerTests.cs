using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheCutHub.Areas.Admin.Controllers;
using TheCutHub.Areas.Admin.Interfaces;
using TheCutHub.Models;
using Xunit;

namespace TheCutHub.Tests.Controllers.Admin
{
    public class AdminWorkingHoursControllerTests
    {
        private readonly Mock<IAdminWorkingHourService> _mockService;
        private readonly WorkingHoursController _controller;

        public AdminWorkingHoursControllerTests()
        {
            _mockService = new Mock<IAdminWorkingHourService>();
            _controller = new WorkingHoursController(_mockService.Object);
        }

        private WorkingHour GetSampleHour(int id = 1) => new WorkingHour { Id = id, BarberId = 5 };

        [Fact]
        public async Task Index_ReturnsView_WithBarbers()
        {
            var barbers = new List<Barber>
    {
        new Barber { Id = 1, FullName = "John" },
        new Barber { Id = 2, FullName = "Alex" }
    };

            _mockService.Setup(s => s.GetAllBarbersAsync()).ReturnsAsync(barbers);

            var result = await _controller.Index();


            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Barber>>(viewResult.Model);
            Assert.Equal(barbers.Count, model.Count);
            Assert.Equal(barbers[0].FullName, model[0].FullName);
            Assert.Equal(barbers[1].FullName, model[1].FullName);
        }


        [Fact]
        public async Task Details_ValidId_ReturnsView()
        {
            var wh = GetSampleHour();
            _mockService.Setup(s => s.GetByIdAsync(wh.Id)).ReturnsAsync(wh);

            var result = await _controller.Details(wh.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(wh, viewResult.Model);
        }

        [Fact]
        public async Task Details_NullId_ReturnsNotFound()
        {
            var result = await _controller.Details(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(42)).ReturnsAsync((WorkingHour)null!);

            var result = await _controller.Details(42);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsView()
        {
            var wh = GetSampleHour();
            _controller.ModelState.AddModelError("Day", "Required");

            var result = await _controller.Create(wh);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(wh, view.Model);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var wh = GetSampleHour();

            var result = await _controller.Create(wh);

            _mockService.Verify(s => s.CreateAsync(wh), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Get_ValidId_ReturnsView()
        {
            var wh = GetSampleHour();
            _mockService.Setup(s => s.GetByIdAsync(wh.Id)).ReturnsAsync(wh);

            var result = await _controller.Edit(wh.Id);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(wh, view.Model);
        }

        [Fact]
        public async Task Edit_Get_NullId_ReturnsNotFound()
        {
            var result = await _controller.Edit(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((WorkingHour)null!);

            var result = await _controller.Edit(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_UpdateFails_ReturnsNotFound()
        {
            var wh = GetSampleHour();
            _mockService.Setup(s => s.UpdateAsync(wh)).ReturnsAsync(false);

            var result = await _controller.Edit(wh.Id, wh);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidUpdate_RedirectsToEditByBarber()
        {
            var wh = GetSampleHour();
            _mockService.Setup(s => s.UpdateAsync(wh)).ReturnsAsync(true);

            var result = await _controller.Edit(wh.Id, wh);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("EditByBarber", redirect.ActionName);
            Assert.Equal(wh.BarberId, redirect.RouteValues["id"]);
        }

        [Fact]
        public async Task EditByBarber_ReturnsViewWithWorkingHours()
        {
            var barberId = 10;
            var workingHours = new List<WorkingHour> { GetSampleHour(1), GetSampleHour(2) };

            _mockService.Setup(s => s.GetByBarberIdAsync(barberId)).ReturnsAsync(workingHours);

            var result = await _controller.EditByBarber(barberId);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("EditByBarber", view.ViewName);
            Assert.Equal(workingHours, view.Model);
        }

        [Fact]
        public async Task Delete_Get_NullId_ReturnsNotFound()
        {
            var result = await _controller.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(77)).ReturnsAsync((WorkingHour)null!);
            var result = await _controller.Delete(77);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_ValidId_ReturnsView()
        {
            var wh = GetSampleHour();
            _mockService.Setup(s => s.GetByIdAsync(wh.Id)).ReturnsAsync(wh);

            var result = await _controller.Delete(wh.Id);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(wh, view.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(s => s.DeleteAsync(55)).ReturnsAsync(false);

            var result = await _controller.DeleteConfirmed(55);
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
