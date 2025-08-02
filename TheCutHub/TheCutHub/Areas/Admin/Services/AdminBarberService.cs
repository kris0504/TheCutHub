using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Services
{
    public class AdminBarberService : IAdminBarberService
    {
        private readonly ApplicationDbContext _context;

        public AdminBarberService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TheCutHub.Models.Barber>> GetAllAsync()
        {
            return await _context.Barbers.AsNoTracking().ToListAsync();
        }

        public async Task<TheCutHub.Models.Barber?> GetByIdAsync(int id)
        {
            return await _context.Barbers.FindAsync(id);
        }

        public async Task CreateAsync(TheCutHub.Models.Barber barber)
        {
            _context.Barbers.Add(barber);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TheCutHub.Models.Barber barber)
        {
            _context.Barbers.Update(barber);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var barber = await _context.Barbers.FindAsync(id);
            if (barber == null) return false;

            _context.Barbers.Remove(barber);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
