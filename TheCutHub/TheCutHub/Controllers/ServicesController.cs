using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheCutHub.Models;
using TheCutHub.Services;

namespace TheCutHub.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var services = await _serviceService.GetAllAsync();
            return View(services);
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var service = await _serviceService.GetByIdAsync(id.Value);
            if (service == null) return NotFound();

            return View(service);
        }

        // GET: Services/Create
        public IActionResult Create() => View();

        // POST: Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,DurationMinutes")] Service service)
        {
            if (!ModelState.IsValid)
                return View(service);

            await _serviceService.CreateAsync(service);
            return RedirectToAction(nameof(Index));
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var service = await _serviceService.GetByIdAsync(id.Value);
            if (service == null) return NotFound();

            return View(service);
        }

        // POST: Services/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,DurationMinutes")] Service service)
        {
            if (id != service.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(service);

            try
            {
                await _serviceService.UpdateAsync(service);
            }
            catch
            {
                if (!_serviceService.Exists(service.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var service = await _serviceService.GetByIdAsync(id.Value);
            if (service == null) return NotFound();

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _serviceService.DeleteAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}