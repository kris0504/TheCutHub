using TheCutHub.Areas.Admin.Interfaces;
using TheCutHub.Data;
using TheCutHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TheCutHub.Areas.Admin.Services
{
    public class AdminWorkingHourService : IAdminWorkingHourService
    {
        private readonly ApplicationDbContext _context;

        public AdminWorkingHourService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TheCutHub.Models.Barber>> GetAllBarbersAsync()
        {
            return await _context.Barbers.ToListAsync();
        }

        public async Task<WorkingHour?> GetByIdAsync(int id)
        {
            return await _context.WorkingHours
                .Include(w => w.Barber)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<WorkingHour>> GetByBarberIdAsync(int barberId)
        {
            return await _context.WorkingHours
                .Where(w => w.BarberId == barberId)
                .OrderBy(w => w.Day)
                .ToListAsync();
        }

        public async Task CreateAsync(WorkingHour workingHour)
        {
            _context.WorkingHours.Add(workingHour);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(WorkingHour workingHour)
        {
            var existing = await _context.WorkingHours.FindAsync(workingHour.Id);
            if (existing == null) return false;

            existing.StartTime = workingHour.StartTime;
            existing.EndTime = workingHour.EndTime;
            existing.IsWorking = workingHour.IsWorking;
            existing.SlotIntervalInMinutes = workingHour.SlotIntervalInMinutes;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var workingHour = await _context.WorkingHours.FindAsync(id);
            if (workingHour == null) return false;

            _context.WorkingHours.Remove(workingHour);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
