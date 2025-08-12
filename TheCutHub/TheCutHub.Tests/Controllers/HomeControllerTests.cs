using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCutHub.Controllers;
using TheCutHub.Models.ViewModels;

namespace TheCutHub.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_Should_Return_View()
        {
            var logger = Mock.Of<ILogger<HomeController>>();
            var controller = new HomeController(logger);
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_Should_Return_View()
        {
            var logger = Mock.Of<ILogger<HomeController>>();
            var controller = new HomeController(logger);
            var result = controller.Privacy();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_Should_Return_View_With_ErrorViewModel()
        {
            var logger = Mock.Of<ILogger<HomeController>>();
            var controller = new HomeController(logger);

            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test-request-id";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = controller.Error();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(view.Model);
            Assert.Equal("test-request-id", model.RequestId);
        }

    }

}
