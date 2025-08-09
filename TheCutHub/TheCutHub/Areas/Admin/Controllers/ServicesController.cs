using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Models;
using X.PagedList;
namespace TheCutHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class ServicesController : Controller
    {
        private readonly IAdminServiceService _serviceService;

        public ServicesController(IAdminServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            const int pageSize = 5;
            var pagedList = await _serviceService.GetPagedAsync(search, page, pageSize);

            ViewBag.Search = search;             
            return View(pagedList);              
        }


        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if (!ModelState.IsValid)
                return View(service);

            await _serviceService.CreateAsync(service);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null) return NotFound();

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(service);

            await _serviceService.UpdateAsync(service);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null) return NotFound();

            return View(service);
        }

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
