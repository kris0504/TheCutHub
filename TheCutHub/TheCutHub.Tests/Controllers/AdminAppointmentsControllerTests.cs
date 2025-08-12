using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TheCutHub.Areas.Admin.Controllers;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Models;
using Xunit;
using X.PagedList;

namespace TheCutHub.Tests.Controllers.Admin
{
	public class AdminAppointmentsControllerTests
	{
		[Fact]
		public async Task Index_Should_Call_GetPaged_And_Return_View()
		{
			var list = new List<Appointment>
			{
				new Appointment { Id = 1 },
				new Appointment { Id = 2 }
			};
			IPagedList<Appointment> paged =
				new StaticPagedList<Appointment>(list, pageNumber: 1, pageSize: 10, totalItemCount: list.Count);

			var service = new Mock<IAdminAppointmentService>();
			service.Setup(s => s.GetPagedAsync(null, null, "date_desc", 1, 10))
				   .ReturnsAsync(paged);

			var controller = new AppointmentsController(service.Object);
			var result = await controller.Index(null, null);
			var view = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IPagedList<Appointment>>(view.Model);
			Assert.Equal(2, model.Count);

			service.Verify(s => s.GetPagedAsync(null, null, "date_desc", 1, 10), Times.Once);
		}

		[Fact]
		public async Task Index_Should_Pass_Through_Filters_And_Set_ViewBag()
		{ 
			var client = "john";
			var date = new DateOnly(2025, 8, 1);
			var sort = "client_asc";
			var page = 3;

			var list = new List<Appointment> { new Appointment { Id = 7 } };
			IPagedList<Appointment> paged =
				new StaticPagedList<Appointment>(list, pageNumber: page, pageSize: 10, totalItemCount: list.Count);

			var service = new Mock<IAdminAppointmentService>();
			service.Setup(s => s.GetPagedAsync(
							It.Is<string?>(c => c == client),
							It.Is<DateOnly?>(d => d.HasValue && d.Value == date),
							It.Is<string>(srt => srt == sort),
							It.Is<int>(p => p == page),
							It.Is<int>(ps => ps == 10)))
				   .ReturnsAsync(paged);

			var controller = new AppointmentsController(service.Object);

			var result = await controller.Index(client, date, sort, page);

			var view = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IPagedList<Appointment>>(view.Model);
			Assert.Single(model);
			Assert.Equal(7, model[0].Id);

			Assert.Equal(client, view.ViewData["Client"]);
			Assert.Equal("2025-08-01", view.ViewData["Date"]);
			Assert.Equal(sort, view.ViewData["Sort"]);

			service.VerifyAll();
		}
	}
}
