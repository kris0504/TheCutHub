using TheCutHub.Data;
using Microsoft.EntityFrameworkCore;

namespace TheCutHub.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSpan>> GetAvailableSlotsAsync(DateTime date, TimeSpan serviceDuration, int barberId)
        {
            var dayOfWeek = date.DayOfWeek;

            var workingHour = await _context.WorkingHours
    .FirstOrDefaultAsync(w => w.Day == dayOfWeek && w.IsWorking && w.BarberId == barberId);
            if (workingHour == null)
            {
                return new List<TimeSpan>();
            }
            
            var appointments = await _context.Appointments
     .Where(a => a.Date.Date == date.Date && a.BarberId == barberId)
     .Include(a => a.User)
     .Include(a => a.Service)
     .Select(a => new
     {
         a.TimeSlot,
         Duration = a.Service.DurationMinutes
     })
     .ToListAsync();

            var slots = new List<TimeSpan>();
            var current = workingHour.StartTime;

            while (current + serviceDuration <= workingHour.EndTime)
            {
                bool isOccupied = appointments.Any(a =>
                    current < a.TimeSlot + TimeSpan.FromMinutes(a.Duration) &&
                    current + serviceDuration > a.TimeSlot);

                if (!isOccupied)
                    slots.Add(current);
                var interval = TimeSpan.FromMinutes(
                 workingHour.SlotIntervalInMinutes > 0 ? workingHour.SlotIntervalInMinutes : 30 );

                if (workingHour == null || workingHour.StartTime >= workingHour.EndTime)
                    return new List<TimeSpan>();

                current = current.Add(interval);
                

            }
            if (workingHour == null || workingHour.StartTime >= workingHour.EndTime)
                return new List<TimeSpan>();

            return slots;
        }
    }

}
