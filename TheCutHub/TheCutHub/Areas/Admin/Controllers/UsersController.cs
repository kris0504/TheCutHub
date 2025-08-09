using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheCutHub.Areas.Admin.Services;
using X.PagedList;
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

        public async Task<IActionResult> Index(string role, string search, int? page)
        {
            var users = await _userService.GetAllUsersAsync();
            var userRoles = await _userService.GetUserRolesMapAsync();

            if (!string.IsNullOrEmpty(role))
            {
                if (role == "User")
                {
                    users = users.Where(u => !userRoles.ContainsKey(u.Id) || userRoles[u.Id].Count == 0).ToList();
                }
                else
                {
                    users = users.Where(u => userRoles.ContainsKey(u.Id) && userRoles[u.Id].Contains(role)).ToList();
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                users = users.Where(u =>
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLower().Contains(search))
                ).ToList();
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.CurrentRole = role;
            ViewBag.SearchTerm = search;

            int pageSize = 10;
            int pageNumber = page ?? 1;

            return View(users.ToPagedList(pageNumber, pageSize));
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
