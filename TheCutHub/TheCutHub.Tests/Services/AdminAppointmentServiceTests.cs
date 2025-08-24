using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Areas.Admin.Services;
using Xunit;

namespace TheCutHub.Tests.Services.Admin
{
    public class AdminAppointmentServiceTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        //[Fact]
        //public async Task GetAllAsync_Should_Return_Appointments_With_Includes()
        //{
            
        //    var context = GetInMemoryContext();
        //    var service = new AdminAppointmentService(context);

        //    var user = new ApplicationUser { Id = "u1", UserName = "user1" };
        //    var barber = new TheCutHub.Models.Barber { Id = 1, UserId = "b1", FullName = "Barber Name" };
        //    var svc = new Service { Id = 1, Name = "Haircut", DurationMinutes = 30 };

        //    var appointment = new Appointment
        //    {
        //        Id = 1,
        //        UserId = user.Id,
        //        BarberId = barber.Id,
        //        ServiceId = svc.Id,
        //        Date = DateTime.Today,
        //        TimeSlot = TimeSpan.FromHours(10),
        //        User = user,
        //        Barber = barber,
        //        Service = svc
        //    };

        //    context.Users.Add(user);
        //    context.Barbers.Add(barber);
        //    context.Services.Add(svc);
        //    context.Appointments.Add(appointment);
        //    await context.SaveChangesAsync();

            
        //    var result = await service.GetAllAsync();

            
        //    Assert.Single(result);
        //    var res = result.First();
        //    Assert.Equal("user1", res.User.UserName);
        //    Assert.Equal("Barber Name", res.Barber.FullName);
        //    Assert.Equal("Haircut", res.Service.Name);
        //}
    }
}
