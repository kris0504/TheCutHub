using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;
using TheCutHub.Services;
using X.PagedList;

namespace TheCutHub.Controllers
{
    [Authorize] 
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(
            IAppointmentService appointmentService,
            UserManager<ApplicationUser> userManager)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = _userManager.GetUserId(User);
            var appointments = await _appointmentService.GetAppointmentsByUserAsync(userId, page, 10);
            return View(appointments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _appointmentService.GetByIdAsync(id.Value);
            if (appointment == null) return NotFound();

            return View(appointment);
        }

        [HttpGet]
        public IActionResult Create(DateTime? date, int? serviceId)
        {
            ViewBag.ServiceId = new SelectList(_appointmentService.GetServices(), "Id", "Name", serviceId);
            ViewBag.BarberId = new SelectList(_appointmentService.GetBarbers(), "Id", "FullName");

            var model = new CreateAppointmentInputModel
            {
                Date = date ?? DateTime.Today
            };

            ViewBag.SelectedDate = date;
            return View(model);
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetSlots([FromQuery] string date, int serviceId, int barberId)
        {
            if (string.IsNullOrWhiteSpace(date) ||
                !DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                         DateTimeStyles.None, out var d))
            {
                return BadRequest(new { error = "Invalid date. Use yyyy-MM-dd." });
            }
            if (serviceId == 0 || barberId == 0)
                return BadRequest(new { error = "Service or Barber not selected" });

            var service = await _appointmentService.GetServiceByIdAsync(serviceId);
            if (service == null)
                return NotFound(new { error = $"Service {serviceId} not found." });

            var dt = d.ToDateTime(new TimeOnly(0, 0));
            var slots = await _appointmentService.GetAvailableSlotsAsync(
                dt, TimeSpan.FromMinutes(service.DurationMinutes), barberId);

            var formatted = slots.Select(s => s.ToString(@"hh\:mm")).ToList();
            return Json(formatted); 
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentInputModel input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ServiceId = new SelectList(_appointmentService.GetServices(), "Id", "Name", input.ServiceId);
                ViewBag.BarberId = new SelectList(_appointmentService.GetBarbers(), "Id", "FullName", input.BarberId);
                return View(input);
            }

         
            var slotFree = await _appointmentService.IsSlotFreeAsync(input.BarberId, input.Date, input.TimeSlot, input.ServiceId);
            if (!slotFree)
            {
                ModelState.AddModelError(nameof(input.TimeSlot), "This timeslot is already taken.");
                ViewBag.ServiceId = new SelectList(_appointmentService.GetServices(), "Id", "Name", input.ServiceId);
                ViewBag.BarberId = new SelectList(_appointmentService.GetBarbers(), "Id", "FullName", input.BarberId);
                return View(input);
            }

            var userId = _userManager.GetUserId(User)!;
            await _appointmentService.CreateAsync(userId, input.Date, input.TimeSlot, input.BarberId, input.ServiceId, input.Notes);

            TempData["Success"] = "Successfully created an appointment.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _appointmentService.GetByIdAsync(id.Value);
            if (appointment == null) return NotFound();

            ViewBag.BarberId = new SelectList(_appointmentService.GetBarbers(), "Id", "FullName", appointment.BarberId);
            ViewBag.ServiceId = new SelectList(_appointmentService.GetServices(), "Id", "Name", appointment.ServiceId);
            ViewBag.UserId = appointment.UserId;

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.BarberId = new SelectList(_appointmentService.GetBarbers(), "Id", "FullName", appointment.BarberId);
                ViewBag.ServiceId = new SelectList(_appointmentService.GetServices(), "Id", "Name", appointment.ServiceId);
                ViewBag.UserId = appointment.UserId;
                return View(appointment);
            }

           

            await _appointmentService.EditAsync(appointment);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var appointment = await _appointmentService.GetByIdAsync(id.Value);
            if (appointment == null || appointment.UserId != userId) return NotFound();

            return View(appointment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _appointmentService.DeleteAsync(id, userId);
            if (!result) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
