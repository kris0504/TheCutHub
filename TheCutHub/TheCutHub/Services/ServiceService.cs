using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;

namespace TheCutHub.Services
{
    public class ServiceService : IServiceService
    {
        private readonly ApplicationDbContext _context;

        public ServiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Service>> GetAllAsync()
        {
            return await _context.Services
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _context.Services
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task CreateAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Services.Any(s => s.Id == id);
        }
    }
}
