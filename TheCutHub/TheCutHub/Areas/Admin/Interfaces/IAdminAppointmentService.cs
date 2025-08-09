using System.Collections.Generic;
using System.Threading.Tasks;
using TheCutHub.Models;
using X.PagedList;

namespace TheCutHub.Areas.Admin.Services
{
    public interface IAdminAppointmentService
    {
        Task<IPagedList<Appointment>> GetPagedAsync(
      string? clientEmail,
      DateOnly? date,
      string sort,
      int page,
      int pageSize);
        Task<List<Appointment>> GetAllAsync();
    }
}