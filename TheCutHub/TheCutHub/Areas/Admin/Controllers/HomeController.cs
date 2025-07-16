using Microsoft.AspNetCore.Mvc;

namespace TheCutHub.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
