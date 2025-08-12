using TheCutHub.Models;
using X.PagedList;

namespace TheCutHub.Areas.Barber.Interfaces
{
    public interface IBarberAppointmentService
    {
        Task<int?> GetBarberIdByUserIdAsync(string userId);
        Task<IPagedList<Appointment>> GetAppointmentsByBarberAsync(int barberId, int page, int pageSize);

    }
}
