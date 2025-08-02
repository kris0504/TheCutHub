using System.Collections.Generic;
using System.Threading.Tasks;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Services
{
    public interface IAdminUserService
    {
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<IDictionary<string, IList<string>>> GetUserRolesMapAsync();
        Task<bool> MakeBarberAsync(string userId);
        Task<bool> RemoveBarberAsync(string userId);
    }
}
