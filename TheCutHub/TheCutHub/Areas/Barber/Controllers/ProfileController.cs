using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Areas.Barber.Services;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;

namespace TheCutHub.Areas.Barber.Controllers
{
    [Area("Barber")]
    [Authorize(Roles = "Barber,Administrator")]
    public class ProfileController : Controller
    {
        private readonly IBarberProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(IBarberProfileService profileService, UserManager<ApplicationUser> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }


        public async Task<IActionResult> Edit(string? id = null)
        {
            string userId;

            if (id != null)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) return NotFound();
                userId = user.Id;
            }
            else
            {
                userId = _userManager.GetUserId(User);
            }

            var model = await _profileService.GetProfileAsync(userId);
            if (model == null) return NotFound();

            ViewBag.BarberId = (await _profileService.GetProfileAsync(userId))?.WorkImages?.FirstOrDefault()?.BarberId;
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BarberProfileEditViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                model.WorkImages = (await _profileService.GetProfileAsync(user.Id))?.WorkImages ?? [];
                return View(model);
            }

            var success = await _profileService.UpdateProfileAsync(user.Id, model);
            if (!success)
            {
                TempData["Fail"] = "Profile not found.";
                return RedirectToAction(nameof(Edit));
            }

            TempData["Success"] = "Profile successfully updated.";
            return RedirectToAction(nameof(Edit));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWorkImage(string? id, AddWorkImageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Form contains invalid data.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            var targetUserId = !string.IsNullOrEmpty(id)
                ? id
                : _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(targetUserId))
                return Challenge();

            var success = await _profileService.AddWorkImageAsync(targetUserId, model);
            TempData[success ? "Success" : "Fail"] =
                success ? "Image was successfully added!" : "Only JPG, PNG and WebP files are allowed.";

            return RedirectToAction(nameof(Edit), new { id = targetUserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkImage(int id, string? userId)
        {
            var targetUserId = !string.IsNullOrEmpty(userId)
                ? userId
                : _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(targetUserId))
                return Challenge();

            var success = await _profileService.DeleteWorkImageAsync(targetUserId, id);
            TempData[success ? "Success" : "Fail"] = success ? "Image successfully deleted." : "Image not found.";

            return RedirectToAction(nameof(Edit), new { id = targetUserId });
        }
    }
}
