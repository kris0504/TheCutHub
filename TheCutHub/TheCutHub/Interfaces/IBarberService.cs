using TheCutHub.Models;

public interface IBarberService
{
    Task<List<Barber>> GetAllAsync();
    Task<Barber?> GetByIdAsync(int id);
    Task<Barber?> GetDetailsAsync(int id);
    Task CreateAsync(Barber barber);
    Task UpdateAsync(Barber barber);
    Task<bool> DeleteAsync(int id);
    bool Exists(int id);
}
