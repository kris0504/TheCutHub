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

    }
}