using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using X.PagedList;

namespace TheCutHub.Areas.Admin.Services
{
    public class AdminAppointmentService : IAdminAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AdminAppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IPagedList<Appointment>> GetPagedAsync(
    string? clientEmail, DateOnly? date, string sort,
    int page, int pageSize)
        {
            var q = _context.Appointments
                            .Include(a => a.User)
                            .Include(a => a.Barber)
                            .Include(a => a.Service)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(clientEmail))
                q = q.Where(a => a.User.Email.Contains(clientEmail));

            if (date.HasValue)
            {
              
                var targetDate = date.Value.ToDateTime(TimeOnly.MinValue);

              
                q = q.Where(a => a.AppointmentDateTime.Date == targetDate.Date);
            }



            var list = await q.AsNoTracking().ToListAsync();

            list = sort switch
            {
                "date_desc" => list.OrderByDescending(a => a.AppointmentDateTime).ToList(),
                "date" => list.OrderBy(a => a.AppointmentDateTime).ToList(),
                "client" => list.OrderBy(a => a.User.Email).ToList(),
                "client_desc" => list.OrderByDescending(a => a.User.Email).ToList(),
                _ => list.OrderByDescending(a => a.AppointmentDateTime).ToList()
            };

            return list.ToPagedList(page, pageSize);
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
