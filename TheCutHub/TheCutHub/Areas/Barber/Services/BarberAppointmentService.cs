using Microsoft.EntityFrameworkCore;
using TheCutHub.Areas.Barber.Interfaces;
using TheCutHub.Data;
using TheCutHub.Models;
using X.PagedList;

namespace TheCutHub.Areas.Barber.Services
{
    public class BarberAppointmentService : IBarberAppointmentService
    {
        private readonly ApplicationDbContext _context;
        public BarberAppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int?> GetBarberIdByUserIdAsync(string userId)
        {
            return await _context.Barbers
                .Where(b => b.UserId == userId)
                .Select(b => (int?)b.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<IPagedList<Appointment>> GetAppointmentsByBarberAsync(int barberId, int page, int pageSize)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .Where(a => a.BarberId == barberId)
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.TimeSlot)
                .ToPagedListAsync(page, pageSize);
        }

    }
}
