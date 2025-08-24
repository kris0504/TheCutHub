using TheCutHub.Models;
using X.PagedList;

namespace TheCutHub.Services
{
    public interface IAppointmentService
    {
        IEnumerable<Barber> GetBarbers();
        IEnumerable<Service> GetServices();
        Task<Service?> GetServiceByIdAsync(int id);
        Task<IPagedList<Appointment>> GetAppointmentsByUserAsync(string userId, int page, int pageSize);
        Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(string userId);
        Task<Appointment?> GetByIdAsync(int id);
        Task CreateAsync(Appointment appointment);
        Task EditAsync(Appointment appointment);
        Task<bool> DeleteAsync(int id, string userId);
        Task<List<TimeSpan>> GetAvailableSlotsAsync(DateOnly date, TimeSpan serviceDuration, int barberId);


        Task<bool> IsSlotFreeAsync(int barberId, DateOnly date, TimeSpan requestedSlot, int serviceId);
        Task CreateAsync(string userId, DateOnly date, TimeSpan timeSlot, int barberId, int serviceId, string? notes);
    }
}
