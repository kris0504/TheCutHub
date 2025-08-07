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



        
    }
}