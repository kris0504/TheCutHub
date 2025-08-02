using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Services
{
    public class AdminAppointmentService : IAdminAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AdminAppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .ToListAsync();
        }
    }
}
