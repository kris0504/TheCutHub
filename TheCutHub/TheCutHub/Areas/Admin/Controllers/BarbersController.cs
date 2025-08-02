using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class BarbersController : Controller
    {
        private readonly IAdminBarberService _barberService;

        public BarbersController(IAdminBarberService barberService)
        {
            _barberService = barberService;
        }

        public async Task<IActionResult> Index()
        {
            var barbers = await _barberService.GetAllAsync();
            return View(barbers);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TheCutHub.Models.Barber barber)
        {
            if (!ModelState.IsValid)
                return View(barber);

            await _barberService.CreateAsync(barber);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var barber = await _barberService.GetByIdAsync(id);
            if (barber == null) return NotFound();

            return View(barber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TheCutHub.Models.Barber barber)
        {
            if (id != barber.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(barber);

            await _barberService.UpdateAsync(barber);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var barber = await _barberService.GetByIdAsync(id);
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
