using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheCutHub.Areas.Admin.Services;

namespace TheCutHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AppointmentsController : Controller
    {
        private readonly IAdminAppointmentService _appointmentService;

        public AppointmentsController(IAdminAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return View(appointments);
        }
    }
}