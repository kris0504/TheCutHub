using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class WorkingHoursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkingHoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkingHours.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var workingHour = await _context.WorkingHours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workingHour == null) return NotFound();

            return View(workingHour);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Day,StartTime,EndTime,IsWorking")] WorkingHour workingHour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workingHour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workingHour);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var workingHour = await _context.WorkingHours.FindAsync(id);
            if (workingHour == null) return NotFound();

            return View(workingHour);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Day,StartTime,EndTime,IsWorking")] WorkingHour workingHour)
        {
            if (id != workingHour.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workingHour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.WorkingHours.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workingHour);
        }

        // GET: Admin/WorkingHours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var workingHour = await _context.WorkingHours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workingHour == null) return NotFound();

            return View(workingHour);
        }

        // POST: Admin/WorkingHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workingHour = await _context.WorkingHours.FindAsync(id);
            _context.WorkingHours.Remove(workingHour!);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
