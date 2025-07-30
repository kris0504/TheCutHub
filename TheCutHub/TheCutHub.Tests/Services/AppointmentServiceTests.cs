using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Services;
using Xunit;

public class AppointmentServiceTests
{
    [Fact]
    public async Task GetAvailableSlotsAsync_ReturnsCorrectSlots()
    {
  
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var barber = new Barber { Id = 1, FullName = "Test Barber" };
        var service = new Service { Id = 1, DurationMinutes = 30 };

        var today = DateTime.Today.AddDays(1);
        var workingHour = new WorkingHour
        {
            Id = 1,
            Barber = barber,
            BarberId = barber.Id,
            Day = today.DayOfWeek,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            IsWorking = true
        };

        context.Barbers.Add(barber);
        context.Services.Add(service);
        context.WorkingHours.Add(workingHour);
        await context.SaveChangesAsync();

        var appointmentService = new AppointmentService(context);

        
        var result = await appointmentService.GetAvailableSlotsAsync(
            today,
            TimeSpan.FromMinutes(service.DurationMinutes),
            barber.Id
        );


        Assert.Equal(4, result.Count);
        Assert.Contains(new TimeSpan(10, 0, 0), result);
    }
}
