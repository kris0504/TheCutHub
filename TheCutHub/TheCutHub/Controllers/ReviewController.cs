using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;
using TheCutHub.Services;

namespace TheCutHub.Controllers
{
    [Authorize]
    [Area("Barber")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(
            IReviewService reviewService,
            UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Fail"] = "Invalid review.";
                return RedirectToAction("Details", "Barbers", new { id = model.BarberId });
            }

            var user = await _userManager.GetUserAsync(User);
            await _reviewService.AddAsync(
                model.BarberId,
                user.Id,
                model.Comment,
                model.Rating,
                DateTime.UtcNow);

            TempData["Success"] = "Review is successfully added!";
            return RedirectToAction("Details", "Barbers", new { id = model.BarberId, area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
           
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null)
                return NotFound();


            var currentUserName = User.Identity?.Name;
            var isBarber = review.Barber?.User?.UserName == currentUserName;
            var isAdmin = User.IsInRole("Administrator");

            if (!isBarber && !isAdmin)
                return Forbid();

    
            await _reviewService.DeleteAsync(review);

            return RedirectToAction("Details", "Barbers", new { id = review.BarberId, area = "" });
        }
    }
}
