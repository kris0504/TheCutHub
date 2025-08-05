using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Models;
using TheCutHub.Services;
using X.PagedList;


namespace TheCutHub.Controllers
{
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

        // GET: Appointments

        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = _userManager.GetUserId(User);
            var appointments = await _appointmentService.GetAppointmentsByUserAsync(userId, page, 10);
            return View(appointments);
        }


        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _appointmentService.GetByIdAsync(id.Value);
            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create(DateTime? date)
        {
            ViewBag.ServiceId = new SelectList(_appointmentService.GetServices(), "Id", "Name");
            ViewBag.BarberId = new SelectList(_appointmentService.GetBarbers(), "Id", "FullName");

            if (date.HasValue)
            {
                ViewBag.SelectedDate = date.Value;
               
            }

            return View();
        }

        // AJAX: Load available slots
        [HttpGet]
        public async Task<IActionResult> GetSlots(DateTime date, int serviceId, int barberId)
        {
            if (serviceId == 0 || barberId == 0)
                return BadRequest("Service or Barber not selected");

            var service = await _appointmentService.GetServiceByIdAsync(serviceId);
            if (service == null)
                return BadRequest("Invalid service");

            var slots = await _appointmentService.GetAvailableSlotsAsync(date, TimeSpan.FromMinutes(service.DurationMinutes), barberId);

            var formattedSlots = slots.Select(s => s.ToString(@"hh\:mm")).ToList();
            return Json(formattedSlots);
        }


        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DateTime date, TimeSpan timeSlot, int serviceId, int barberId, string notes)
        {
            var userId = _userManager.GetUserId(User);

            var appointment = new Appointment
            {
                Date = date.Date,
                TimeSlot = timeSlot,
                ServiceId = serviceId,
                BarberId = barberId,
                UserId = userId,
                Notes = notes
            };

            await _appointmentService.CreateAsync(appointment);
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Edit/5
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

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _appointmentService.EditAsync(appointment);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BarberId = new SelectList(_appointmentService.GetBarbers(), "Id", "FullName", appointment.BarberId);
            ViewBag.ServiceId = new SelectList(_appointmentService.GetServices(), "Id", "Name", appointment.ServiceId);
            ViewBag.UserId = appointment.UserId;

            return View(appointment);
        }

        // GET: Appointments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var appointment = await _appointmentService.GetByIdAsync(id.Value);
            if (appointment == null || appointment.UserId != userId) return NotFound();

            return View(appointment);
        }

        // POST: Appointments/Delete/5
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
