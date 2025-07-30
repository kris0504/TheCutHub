using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                userRoles[user.Id] = await _userManager.GetRolesAsync(user);
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> MakeBarber(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (!await _userManager.IsInRoleAsync(user, "Barber"))
            {
                await _userManager.AddToRoleAsync(user, "Barber");
            }

            if (!await _context.Barbers.AnyAsync(b => b.UserId == user.Id))
            {
                var barber = new TheCutHub.Models.Barber
                {
                    UserId = user.Id,
                    FullName = user.FullName ?? user.Email
                };

                _context.Barbers.Add(barber);
                await _context.SaveChangesAsync();

                TempData["BarberCreated"] = $"User {user.Email} is already a barber.";
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> RemoveBarber(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

          
            if (await _userManager.IsInRoleAsync(user, "Barber"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Barber");
            }

            
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == user.Id);
            if (barber != null)
            {
                _context.Barbers.Remove(barber);
                await _context.SaveChangesAsync();
            }

            TempData["BarberRemoved"] = $"User {user.Email} is not a barber anymore.";
            return RedirectToAction(nameof(Index));
        }

    }
}