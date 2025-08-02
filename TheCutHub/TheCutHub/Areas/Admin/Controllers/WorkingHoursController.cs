using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheCutHub.Areas.Admin.Interfaces;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class WorkingHoursController : Controller
    {
        private readonly IAdminWorkingHourService _workingHourService;

        public WorkingHoursController(IAdminWorkingHourService workingHourService)
        {
            _workingHourService = workingHourService;
        }

        public async Task<IActionResult> Index()
        {
            var barbers = await _workingHourService.GetAllBarbersAsync();
            return View(barbers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var workingHour = await _workingHourService.GetByIdAsync(id.Value);
            if (workingHour == null) return NotFound();

            return View(workingHour);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkingHour workingHour)
        {
            if (!ModelState.IsValid)
                return View(workingHour);

            await _workingHourService.CreateAsync(workingHour);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var workingHour = await _workingHourService.GetByIdAsync(id.Value);
            if (workingHour == null) return NotFound();

            return View(workingHour);
        }

        public async Task<IActionResult> EditByBarber(int id)
        {
            var workingHours = await _workingHourService.GetByBarberIdAsync(id);
            ViewBag.BarberId = id;
            return View("EditByBarber", workingHours);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkingHour workingHour)
        {
            var updated = await _workingHourService.UpdateAsync(workingHour);
            if (!updated) return NotFound();

            return RedirectToAction("EditByBarber", new { id = workingHour.BarberId });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var workingHour = await _workingHourService.GetByIdAsync(id.Value);
            if (workingHour == null) return NotFound();

            return View(workingHour);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _workingHourService.DeleteAsync(id);
            if (!deleted) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
