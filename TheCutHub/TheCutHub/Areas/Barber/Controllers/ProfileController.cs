using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;

namespace TheCutHub.Areas.Barber.Controllers
{
    [Area("Barber")]
    [Authorize(Roles = "Barber")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET Barber/Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var barber = await _context.Barbers
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.UserId == user.Id);

            if (barber == null) return NotFound();

            var model = new BarberProfileEditViewModel
            {
                FullName = barber.FullName,
                Bio = barber.Bio,
                ProfileImageUrl = barber.ProfileImageUrl,
                //Email = barber.User?.Email
            };
            Console.WriteLine($"[GET] FullName: {barber.FullName}, Bio: {barber.Bio}");

            return View(model);
        }

        // POST Barber/Profile/Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BarberProfileEditViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == user.Id);

            if (barber == null)
            {
                TempData["Fail"] = "Профилът не е намерен.";
                return RedirectToAction(nameof(Edit));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            barber.FullName = model.FullName;
            barber.Bio = model.Bio;
            barber.ProfileImageUrl = model.ProfileImageUrl;


            _context.Barbers.Update(barber);

           
            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"[DEBUG] SaveChanges result = {result}");
            Console.WriteLine($"[DEBUG] Barber.Id = {barber.Id}");
            Console.WriteLine($"[DEBUG] User.Id = {user.Id}");
            Console.WriteLine($"[DEBUG] Incoming ViewModel: FullName = {model.FullName}, Bio = {model.Bio}, ProfileImageUrl = {model.ProfileImageUrl}");

            TempData["Success"] = "Профилът е обновен успешно.";
            return RedirectToAction(nameof(Edit));
        }




    }
}
