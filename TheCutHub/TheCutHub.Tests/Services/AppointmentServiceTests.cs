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
    public async Task GetAvailableSlotsAsync_SkipsTakenSlots()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var fakeUser = new ApplicationUser
        {
            Id = "user-1",
            UserName = "barber@example.com"
        };

        var barber = new Barber
        {
            Id = 1,
            FullName = "Test Barber",
            UserId = fakeUser.Id,
            User = fakeUser
        };

        context.Users.Add(fakeUser); // или context.ApplicationUsers.Add(...) ако имаш DbSet
        context.Barbers.Add(barber);
        var service = new Service
        {
            Id = 1,
            Name = "Haircut",
            DurationMinutes = 30
        };

        var date = DateTime.Today.AddDays(1);

        var workingHour = new WorkingHour
        {
            Id = 1,
            Barber = barber,
            BarberId = barber.Id,
            Day = date.DayOfWeek,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            IsWorking = true
        };

        var client = new ApplicationUser
        {
            Id = "test-user-id",
            UserName = "client@example.com"
        };

        var existingAppointment = new Appointment
        {
            Id = 1,
            UserId = client.Id,
            User = client,
            BarberId = barber.Id,
            Barber = barber,
            ServiceId = service.Id,
            Service = service,
            Date = date,
            TimeSlot = new TimeSpan(10, 30, 0),
            Notes = "Test note"
        };

        context.Users.Add(client);
        context.Barbers.Add(barber);
        context.Services.Add(service);
        context.WorkingHours.Add(workingHour);
        context.Appointments.Add(existingAppointment);

        await context.SaveChangesAsync();


        var appointmentService = new AppointmentService(context);

       
        var result = await appointmentService.GetAvailableSlotsAsync(
            date,
            TimeSpan.FromMinutes(service.DurationMinutes),
            barber.Id
        );


        Assert.Equal(3, result.Count);
        Assert.DoesNotContain(new TimeSpan(10, 30, 0), result);
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_ReturnsCorrectSlots()
    {
  
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var fakeUser = new ApplicationUser
        {
            Id = "user-1",
            UserName = "barber@example.com"
        };

        var barber = new Barber
        {
            Id = 1,
            FullName = "Test Barber",
            UserId = fakeUser.Id,
            User = fakeUser
        };

        context.Users.Add(fakeUser);
        context.Barbers.Add(barber);

        var service = new Service
        {
            Id = 1,
            Name = "Haircut",
            DurationMinutes = 30
        };

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
