using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Services;

namespace TheCutHub.Controllers
{
    public class BarbersController : Controller
    {
        private readonly IBarberService _barberService;
        private readonly ApplicationDbContext _context;
        public BarbersController(IBarberService barberService, ApplicationDbContext context)
        {
			_context = context;
			_barberService = barberService;
        }

        public async Task<IActionResult> Index()
        {
            var barbers = await _barberService.GetAllAsync();
            return View(barbers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var barber = await _barberService.GetDetailsAsync(id.Value);
            if (barber == null) return NotFound();
            var reviews = await _context.Reviews
        .Where(r => r.BarberId == barber.Id)
        .ToListAsync();

            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            ViewBag.AverageRating = averageRating;
            return View(barber);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,ProfileImageUrl,Bio")] Barber barber)
        {
            if (!ModelState.IsValid)
                return View(barber);

            await _barberService.CreateAsync(barber);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var barber = await _barberService.GetByIdAsync(id.Value);
            if (barber == null) return NotFound();

            return View(barber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,ProfileImageUrl,Bio")] Barber barber)
        {
            if (id != barber.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(barber);

            try
            {
                await _barberService.UpdateAsync(barber);
            }
            catch
            {
                if (!_barberService.Exists(barber.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var barber = await _barberService.GetByIdAsync(id.Value);
            if (barber == null) return NotFound();

            return View(barber);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _barberService.DeleteAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
