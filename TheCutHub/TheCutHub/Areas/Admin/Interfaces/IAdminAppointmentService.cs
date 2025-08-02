using System.Collections.Generic;
using System.Threading.Tasks;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Services
{
    public interface IAdminAppointmentService
    {
        Task<List<Appointment>> GetAllAsync();
    }
}