using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using TheCutHub.Controllers;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;
using TheCutHub.Services;
using X.PagedList;
using Xunit;

namespace TheCutHub.Tests.Controllers
{
    public class AppointmentsControllerTests
    {

		private static Mock<UserManager<ApplicationUser>> MockUserManager()
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
			// Arrange
			var userId = "user1";
			var page = 1;
			var pageSize = 10;

			var mockService = new Mock<IAppointmentService>();
			var mockUserManager = MockUserManager();
			mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

			var list = new List<Appointment> { new Appointment { Id = 1, UserId = userId } };
			IPagedList<Appointment> paged = new StaticPagedList<Appointment>(list, page, pageSize, list.Count);

			mockService
				.Setup(s => s.GetAppointmentsByUserAsync(userId, page, pageSize))
				.ReturnsAsync(paged);

			var controller = new AppointmentsController(mockService.Object, mockUserManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(
						new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "TestAuth"))
				}
			};

			// Act
			var result = await controller.Index(page);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IPagedList<Appointment>>(viewResult.Model);
			Assert.Single(model);
			Assert.Equal(userId, model[0].UserId);

			mockService.Verify(s => s.GetAppointmentsByUserAsync(userId, page, pageSize), Times.Once);
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
            var mockUserManager = MockUserManager();
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
            var mockUserManager = MockUserManager();
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
            var result = controller.Create(null,null);

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

			var userId = "user-123";
			var input = new CreateAppointmentInputModel
			{
				Date = new DateTime(2025, 8, 20),
				TimeSlot = new TimeSpan(14, 30, 0),
				BarberId = 5,
				ServiceId = 7,
				Notes = "Please be gentle"
			};

			var appointmentServiceMock = new Mock<IAppointmentService>();
			appointmentServiceMock
				.Setup(s => s.IsSlotFreeAsync(input.BarberId, input.Date, input.TimeSlot, input.ServiceId))
				.ReturnsAsync(true);
			appointmentServiceMock
				.Setup(s => s.CreateAsync(userId, input.Date, input.TimeSlot, input.BarberId, input.ServiceId, input.Notes))
				.Returns(Task.CompletedTask)
				.Verifiable();

			var userManagerMock = MockUserManager();
			userManagerMock
				.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()))
				.Returns(userId);   

			var controller = new AppointmentsController(appointmentServiceMock.Object, userManagerMock.Object);


			var httpContext = new DefaultHttpContext
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(
					new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "TestAuth"))
			};
			controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
			controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

			var result = await controller.Create(input);

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal(nameof(AppointmentsController.Index), redirect.ActionName);

			
			appointmentServiceMock.Verify(s =>
				s.CreateAsync(userId, input.Date, input.TimeSlot, input.BarberId, input.ServiceId, input.Notes),
				Times.Once);

			Assert.Equal("Successfully created an appointment.", controller.TempData["Success"]);
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
