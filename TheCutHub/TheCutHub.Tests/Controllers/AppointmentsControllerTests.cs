using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using TheCutHub.Controllers;
using TheCutHub.Models;
using TheCutHub.Services;
using Xunit;

namespace TheCutHub.Tests.Controllers
{
    public class AppointmentsControllerTests
    {
        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private ControllerContext GetFakeContextWithUserId(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Index_Should_Return_User_Appointments()
        {
            var mockService = new Mock<IAppointmentService>();
            var mockUserManager = GetMockUserManager();
            mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

            mockService.Setup(s => s.GetAppointmentsByUserIdAsync("user1"))
                .ReturnsAsync(new List<Appointment> { new Appointment { Id = 1, UserId = "user1" } });

            var controller = new AppointmentsController(mockService.Object, mockUserManager.Object);
            controller.ControllerContext = GetFakeContextWithUserId("user1");

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Appointment>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_Should_Return_404_If_Id_Null()
        {
            var controller = new AppointmentsController(null!, null!);
            var result = await controller.Details(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Should_Return_404_If_Not_Found()
        {
            var mockService = new Mock<IAppointmentService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Appointment)null!);
            var controller = new AppointmentsController(mockService.Object, null!);

            var result = await controller.Details(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Should_Return_Appointment_If_Found()
        {
            var mockService = new Mock<IAppointmentService>();
            var appointment = new Appointment { Id = 1, UserId = "user1" };
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(appointment);
            var controller = new AppointmentsController(mockService.Object, null!);

            var result = await controller.Details(1);
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(appointment, view.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_Should_Return_404_If_Delete_Fails()
        {
            var mockService = new Mock<IAppointmentService>();
            mockService.Setup(s => s.DeleteAsync(1, "user1")).ReturnsAsync(false);
            var mockUserManager = GetMockUserManager();
            mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

            var controller = new AppointmentsController(mockService.Object, mockUserManager.Object);
            controller.ControllerContext = GetFakeContextWithUserId("user1");

            var result = await controller.DeleteConfirmed(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_Should_Redirect_If_Successful()
        {
            var mockService = new Mock<IAppointmentService>();
            mockService.Setup(s => s.DeleteAsync(1, "user1")).ReturnsAsync(true);
            var mockUserManager = GetMockUserManager();
            mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

            var controller = new AppointmentsController(mockService.Object, mockUserManager.Object);
            controller.ControllerContext = GetFakeContextWithUserId("user1");

            var result = await controller.DeleteConfirmed(1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void Create_Get_Should_Return_View_With_SelectLists()
        {
            var mockService = new Mock<IAppointmentService>();
            mockService.Setup(s => s.GetBarbers()).Returns(new List<Barber> { new Barber { Id = 1, FullName = "B" } });
            mockService.Setup(s => s.GetServices()).Returns(new List<Service> { new Service { Id = 1, Name = "S" } });

            var controller = new AppointmentsController(mockService.Object, null!);
            var result = controller.Create(null);

            var view = Assert.IsType<ViewResult>(result);
            Assert.NotNull(view);
        }

        [Fact]
        public async Task GetSlots_Should_Return_BadRequest_If_Inputs_Invalid()
        {
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentsController(mockService.Object, null!);

            var result = await controller.GetSlots(DateTime.Today, 0, 0);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_Post_Should_Create_Appointment_And_Redirect()
        {
            var mockService = new Mock<IAppointmentService>();
            var mockUserManager = GetMockUserManager();
            mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user1");

            var controller = new AppointmentsController(mockService.Object, mockUserManager.Object);
            controller.ControllerContext = GetFakeContextWithUserId("user1");

            var result = await controller.Create(DateTime.Today, TimeSpan.FromHours(10), 1, 1, "note");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Get_Should_Return_NotFound_If_Null()
        {
            var controller = new AppointmentsController(null!, null!);
            var result = await controller.Edit(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_Should_Redirect_If_Valid()
        {
            var mockService = new Mock<IAppointmentService>();
            var controller = new AppointmentsController(mockService.Object, null!);

            var appointment = new Appointment { Id = 1, BarberId = 1, ServiceId = 1, UserId = "user1" };
            var result = await controller.Edit(1, appointment);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}
