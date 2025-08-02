using System.Threading.Tasks;
using TheCutHub.Models.ViewModels;

namespace TheCutHub.Areas.Barber.Services
{
    public interface IBarberProfileService
    {
        Task<BarberProfileEditViewModel?> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, BarberProfileEditViewModel model);
        Task<bool> AddWorkImageAsync(string userId, AddWorkImageViewModel model);
        Task<bool> DeleteWorkImageAsync(string userId, int imageId);
    }
}