using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheCutHub.Areas.Admin.Controllers;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Models;

namespace TheCutHub.Tests.Controllers.Admin
{
    public class AdminAppointmentsControllerTests
    {
        [Fact]
        public async Task Index_Should_Return_View_With_Appointments()
        {
            
            var mockService = new Mock<IAdminAppointmentService>();
            mockService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Appointment> {
                new Appointment { Id = 1 },
                new Appointment { Id = 2 }
                });

            var controller = new AppointmentsController(mockService.Object);

            
            var result = await controller.Index();

            
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Appointment>>(view.Model);
            Assert.Equal(2, ((List<Appointment>)model).Count);
        }
    }
}
