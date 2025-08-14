using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheCutHub.Models;

using X.PagedList;
using TheCutHub.Areas.Barber.Interfaces;

namespace TheCutHub.Areas.Barber.Controllers
{
    [Area("Barber")]
    [Authorize(Roles = "Barber")]
    public class BarberAppointmentsController : Controller
    {
        private readonly IBarberAppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BarberAppointmentsController(
            IBarberAppointmentService appointmentService,
            UserManager<ApplicationUser> userManager)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = _userManager.GetUserId(User)!;
            var barberId = await _appointmentService.GetBarberIdByUserIdAsync(userId);
            if (barberId is null)
            {
                TempData["Error"] = "You do not have configured barber profile.";
                return RedirectToAction("Edit", "Profile"); 
            }

            const int pageSize = 10;
            var appts = await _appointmentService.GetAppointmentsByBarberAsync(barberId.Value, page, pageSize);
            return View(appts);
        }
    }
}
