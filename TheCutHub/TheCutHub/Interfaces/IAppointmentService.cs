using TheCutHub.Models;
using X.PagedList;

public interface IAppointmentService
{
    Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(string userId);
    Task<Appointment?> GetByIdAsync(int id);
    Task CreateAsync(Appointment appointment);
    Task EditAsync(Appointment appointment);
    Task<bool> DeleteAsync(int id, string userId);
    Task<List<TimeSpan>> GetAvailableSlotsAsync(DateTime date, TimeSpan serviceDuration, int barberId);
    IEnumerable<Barber> GetBarbers();
    IEnumerable<Service> GetServices();
    Task<Service?> GetServiceByIdAsync(int id);
    Task<IPagedList<Appointment>> GetAppointmentsByUserAsync(string userId, int page, int pageSize);
}
