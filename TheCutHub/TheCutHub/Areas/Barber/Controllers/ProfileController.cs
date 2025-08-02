using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheCutHub.Areas.Barber.Services;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;

namespace TheCutHub.Areas.Barber.Controllers
{
    [Area("Barber")]
    [Authorize(Roles = "Barber")]
    public class ProfileController : Controller
    {
        private readonly IBarberProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(IBarberProfileService profileService, UserManager<ApplicationUser> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = await _profileService.GetProfileAsync(user.Id);
            if (model == null) return NotFound();
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
        public async Task<IActionResult> AddWorkImage(AddWorkImageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Form contains invalid data.";
                return RedirectToAction("Edit");
            }

            var user = await _userManager.GetUserAsync(User);
            var success = await _profileService.AddWorkImageAsync(user.Id, model);
            if (!success)
            {
                TempData["Fail"] = "Only JPG, PNG and WebP files are allowed.";
                return RedirectToAction("Edit");
            }

            TempData["Success"] = "Image was successfully added!";
            return RedirectToAction("Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkImage(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var success = await _profileService.DeleteWorkImageAsync(user.Id, id);
            TempData[success ? "Success" : "Fail"] = success ? "Image successfully deleted." : "Image not found.";
            return RedirectToAction("Edit");
        }
    }
}
