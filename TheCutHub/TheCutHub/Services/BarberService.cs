using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;

public class BarberService : IBarberService
{
    private readonly ApplicationDbContext _context;

    public BarberService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Barber>> GetAllAsync()
    {
        return await _context.Barbers.AsNoTracking().ToListAsync();
    }

    public async Task<Barber?> GetByIdAsync(int id)
    {
        return await _context.Barbers.FindAsync(id);
    }

    public async Task<Barber?> GetDetailsAsync(int id)
    {
        return await _context.Barbers
            .Include(b => b.User)
            .Include(b => b.Appointments)
            .Include(b => b.WorkImages)
            .Include(b => b.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task CreateAsync(Barber barber)
    {
        _context.Barbers.Add(barber);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Barber barber)
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

    public bool Exists(int id)
    {
        return _context.Barbers.Any(b => b.Id == id);
    }
}
