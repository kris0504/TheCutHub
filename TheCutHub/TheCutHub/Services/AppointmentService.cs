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

        public async Task<List<TimeSpan>> GetAvailableSlotsAsync(DateTime date, TimeSpan serviceDuration)
        {
            var dayOfWeek = date.DayOfWeek;

            var workingHour = await _context.WorkingHours
                .FirstOrDefaultAsync(w => w.Day == dayOfWeek && w.IsWorking);

            if (workingHour == null)
                return new List<TimeSpan>();

            var appointments = await _context.Appointments
                .Where(a => a.Date.Date == date.Date)
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

                current = current.Add(TimeSpan.FromMinutes(30));
            }

            return slots;
        }
    }

}
