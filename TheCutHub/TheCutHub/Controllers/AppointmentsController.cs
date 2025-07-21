using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Services;

namespace TheCutHub.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AppointmentService _appointmentService;
        
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(
            ApplicationDbContext context,
            AppointmentService appointmentService,
        UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _appointmentService = appointmentService;
            _userManager = userManager;
        }

        //public AppointmentsController(ApplicationDbContext context)
        //{
        //    _context = context;
        //    _appointmentService = new AppointmentService(context); // още не сме направили DI
        //}

       

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var appointments = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(appointments);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        //public IActionResult Create()
        //{
            
        //    ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "FullName");
        //    ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
        //    ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
        //    return View();
        //}
        public async Task<IActionResult> Create(DateTime? date)
        {
            ViewBag.ServiceId = new SelectList(_context.Services, "Id", "Name");
            ViewBag.BarberId = new SelectList(_context.Barbers, "Id", "FullName");

            if (date.HasValue)
            {
                ViewBag.SelectedDate = date.Value;

                var start = new TimeSpan(9, 0, 0);
                var end = new TimeSpan(18, 0, 0);
                var allSlots = new List<TimeSpan>();

                for (var ts = start; ts < end; ts = ts.Add(TimeSpan.FromMinutes(30)))
                {
                    allSlots.Add(ts);
                }

                var bookedSlots = await _context.Appointments
                    .Where(a => a.Date == date.Value.Date)
                    .Select(a => a.TimeSlot)
                    .ToListAsync();

                
                var availableSlots = allSlots.Except(bookedSlots).ToList();
                ViewBag.Slots = availableSlots;
            }

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetSlots(DateTime date, int serviceId, int barberId)
        {
            if (serviceId == 0 || barberId == 0)
                return BadRequest("Service or Barber not selected");

            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
                return BadRequest("Invalid service");
            
            var slots = await _appointmentService.GetAvailableSlotsAsync(date, TimeSpan.FromMinutes(service.DurationMinutes), barberId);
            return PartialView("_TimeSlotsPartial", slots);
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,UserId,BarberId,ServiceId,AppointmentDateTime,Notes")] Appointment appointment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(appointment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "FullName", appointment.BarberId);
        //    ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
        //    ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", appointment.UserId);
        //    return View(appointment);
        //}
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

            _context.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Appointments");
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "FullName", appointment.BarberId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", appointment.UserId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,BarberId,ServiceId,AppointmentDateTime,Notes")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BarberId"] = new SelectList(_context.Barbers, "Id", "FullName", appointment.BarberId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", appointment.UserId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            var appointment = await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (appointment == null) return NotFound();

            return View(appointment);
        }


        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null) return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
