using TheCutHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace TheCutHub.Areas.Admin.Interfaces
{
    public interface IAdminWorkingHourService
    {
        Task<List<TheCutHub.Models.Barber>> GetAllBarbersAsync();
        Task<WorkingHour?> GetByIdAsync(int id);
        Task<List<WorkingHour>> GetByBarberIdAsync(int barberId);
        Task CreateAsync(WorkingHour workingHour);
        Task<bool> UpdateAsync(WorkingHour workingHour);
        Task<bool> DeleteAsync(int id);
    }
}
