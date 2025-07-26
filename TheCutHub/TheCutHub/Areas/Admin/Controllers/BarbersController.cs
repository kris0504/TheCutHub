using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class BarbersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarbersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var barbers = await _context.Barbers.ToListAsync();
            return View(barbers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TheCutHub.Models.Barber barber)
        {
            if (ModelState.IsValid)
            {
                _context.Barbers.Add(barber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(barber);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var barber = await _context.Barbers.FindAsync(id);
            if (barber == null) return NotFound();

            return View(barber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TheCutHub.Models.Barber barber)
        {
            if (id != barber.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Barbers.Update(barber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(barber);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var barber = await _context.Barbers.FindAsync(id);
            if (barber == null) return NotFound();

            return View(barber);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var barber = await _context.Barbers.FindAsync(id);
            if (barber != null)
            {
                _context.Barbers.Remove(barber);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
