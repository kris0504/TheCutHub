using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace TheCutHub.Tests.Controllers
{
    public class ErrorControllerTests
    {
        [Fact]
        public void HttpStatusCodeHandler_Should_Return_404_View()
        {
            var controller = new ErrorController();
            var result = controller.HttpStatusCodeHandler(404);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error404", view.ViewName);
        }

        [Fact]
        public void HttpStatusCodeHandler_Should_Return_500_View_For_500()
        {
            var controller = new ErrorController();
            var result = controller.HttpStatusCodeHandler(500);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error500", view.ViewName);
        }

        [Fact]
        public void HttpStatusCodeHandler_Should_Default_To_500_View()
        {
            var controller = new ErrorController();
            var result = controller.HttpStatusCodeHandler(403);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error500", view.ViewName);
        }

        [Fact]
        public void Error_Should_Return_500_View()
        {
            var controller = new ErrorController();
            var result = controller.Error();
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error500", view.ViewName);
        }
    }
}