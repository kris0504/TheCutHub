using System.Collections.Generic;
using System.Threading.Tasks;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Services
{
    public interface IAdminBarberService
    {
        Task<List<TheCutHub.Models.Barber>> GetAllAsync();
        Task<TheCutHub.Models.Barber?> GetByIdAsync(int id);
        Task CreateAsync(TheCutHub.Models.Barber barber);
        Task UpdateAsync(TheCutHub.Models.Barber barber);
        Task<bool> DeleteAsync(int id);
    }
}