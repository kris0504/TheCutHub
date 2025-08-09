using System.Collections.Generic;
using System.Threading.Tasks;
using TheCutHub.Models;
using X.PagedList;

namespace TheCutHub.Areas.Admin.Services
{
    public interface IAdminServiceService
    {
        Task<IPagedList<Service>> GetPagedAsync(string? search, int page, int pageSize);
        Task<List<Service>> GetAllAsync();
        Task<Service?> GetByIdAsync(int id);
        Task CreateAsync(Service service);
        Task UpdateAsync(Service service);
        Task<bool> DeleteAsync(int id);
    }
}