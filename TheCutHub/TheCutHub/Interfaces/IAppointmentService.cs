using TheCutHub.Models;

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

}
