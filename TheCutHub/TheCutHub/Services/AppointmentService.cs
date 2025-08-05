using TheCutHub.Data;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Models;
using X.PagedList;

namespace TheCutHub.Services
{
  
        public class AppointmentService : IAppointmentService
        {
            private readonly ApplicationDbContext _context;

            public AppointmentService(ApplicationDbContext context)
            {
                _context = context;
            }
        public IEnumerable<Barber> GetBarbers() => _context.Barbers.AsNoTracking().ToList();

        public IEnumerable<Service> GetServices() => _context.Services.AsNoTracking().ToList();

        public async Task<Service?> GetServiceByIdAsync(int id) =>
            await _context.Services.FindAsync(id);
        public async Task<IPagedList<Appointment>> GetAppointmentsByUserAsync(string userId, int page, int pageSize)
        {
            return await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Date)
                .ToPagedListAsync(page, pageSize);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(string userId)
            {
                return await _context.Appointments
                    .Include(a => a.Barber)
                    .Include(a => a.Service)
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.Date)
                    .ToListAsync();
            }

            public async Task<Appointment?> GetByIdAsync(int id)
            {
                return await _context.Appointments
                    .Include(a => a.Barber)
                    .Include(a => a.Service)
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }

            public async Task CreateAsync(Appointment appointment)
            {
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
            }

            public async Task EditAsync(Appointment appointment)
            {
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
            }

            public async Task<bool> DeleteAsync(int id, string userId)
            {
                var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
                if (appointment == null) return false;

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<List<TimeSpan>> GetAvailableSlotsAsync(DateTime date, TimeSpan serviceDuration, int barberId)
            {
                var dayOfWeek = date.DayOfWeek;

                var workingHour = await _context.WorkingHours
                    .FirstOrDefaultAsync(w => w.Day == dayOfWeek && w.IsWorking && w.BarberId == barberId);

                if (workingHour == null || workingHour.StartTime >= workingHour.EndTime)
                    return new List<TimeSpan>();

                var appointments = await _context.Appointments
                    .Where(a => a.Date.Date == date.Date && a.BarberId == barberId)
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
                        workingHour.SlotIntervalInMinutes > 0 ? workingHour.SlotIntervalInMinutes : 30);

                    current = current.Add(interval);
                }
                    
                return slots;
            }


        }

  
    

}
