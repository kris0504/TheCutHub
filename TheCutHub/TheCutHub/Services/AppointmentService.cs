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
        public async Task CreateAsync(string userId, DateTime date, TimeSpan timeSlot, int barberId, int serviceId, string? notes)
        {
            var appointment = new Appointment
            {
                UserId = userId,
                Date = date.Date,
                TimeSlot = timeSlot,
                BarberId = barberId,
                ServiceId = serviceId,
                Notes = notes
            };

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
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.BarberId == barberId && w.Day == dayOfWeek && w.IsWorking);

            if (workingHour == null || workingHour.StartTime >= workingHour.EndTime)
                return new List<TimeSpan>();

            
            var appointments = await _context.Appointments
                .AsNoTracking()
                .Where(a => a.BarberId == barberId && a.Date.Date == date.Date)
                .Join(_context.Services,
                      a => a.ServiceId,
                      s => s.Id,
                      (a, s) => new
                      {
                          a.TimeSlot,
                          Duration = TimeSpan.FromMinutes(s.DurationMinutes)
                      })
                .ToListAsync();

            var slots = new List<TimeSpan>();
            var current = workingHour.StartTime;

            var intervalMinutes = workingHour.SlotIntervalInMinutes > 0 ? workingHour.SlotIntervalInMinutes : 30;
            var interval = TimeSpan.FromMinutes(intervalMinutes);

            while (current + serviceDuration <= workingHour.EndTime)
            {
                bool isOccupied = appointments.Any(a =>
                    current < a.TimeSlot + a.Duration &&
                    current + serviceDuration > a.TimeSlot);

                if (!isOccupied)
                    slots.Add(current);

                current = current.Add(interval);
            }

            return slots;
        }

        public async Task<bool> IsSlotFreeAsync(int barberId, DateTime date, TimeSpan requestedSlot, int serviceId)
        {
            var service = await _context.Services.AsNoTracking().FirstOrDefaultAsync(s => s.Id == serviceId);
            if (service == null) return false;

            var requestedDuration = TimeSpan.FromMinutes(service.DurationMinutes);
            var requestedEnd = requestedSlot + requestedDuration;

            
            var sameDay = await _context.Appointments
                .AsNoTracking()
                .Where(a => a.BarberId == barberId && a.Date.Date == date.Date)
                .Join(_context.Services,
                      a => a.ServiceId,
                      s => s.Id,
                      (a, s) => new
                      {
                          a.TimeSlot,
                          Duration = TimeSpan.FromMinutes(s.DurationMinutes)
                      })
                .ToListAsync();

            var overlaps = sameDay.Any(a =>
                requestedSlot < a.TimeSlot + a.Duration &&
                requestedEnd > a.TimeSlot);

            return !overlaps;
        }


    }
}