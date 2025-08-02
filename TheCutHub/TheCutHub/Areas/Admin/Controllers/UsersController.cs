using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheCutHub.Areas.Admin.Services;

namespace TheCutHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly IAdminUserService _userService;

        public UsersController(IAdminUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var userRoles = await _userService.GetUserRolesMapAsync();

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeBarber(string userId)
        {
            var result = await _userService.MakeBarberAsync(userId);
            if (!result) return NotFound();

            TempData["BarberCreated"] = "User is now a barber.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBarber(string userId)
        {
            var result = await _userService.RemoveBarberAsync(userId);
            if (!result) return NotFound();

            TempData["BarberRemoved"] = "User is no longer a barber.";
            return RedirectToAction(nameof(Index));
        }
    }
}
