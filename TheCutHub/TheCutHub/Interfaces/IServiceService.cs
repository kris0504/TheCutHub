using System.Collections.Generic;
using System.Threading.Tasks;
using TheCutHub.Models;

namespace TheCutHub.Services
{
    public interface IServiceService
    {
        Task<List<Service>> GetAllAsync();
        Task<Service?> GetByIdAsync(int id);
        Task CreateAsync(Service service);
        Task UpdateAsync(Service service);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);
    }
}

    