using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheCutHub.Data;
using TheCutHub.Models.ViewModels;
using TheCutHub.Models;
using Microsoft.EntityFrameworkCore;

namespace TheCutHub.Controllers
{
    [Authorize]
    [Area("Barber")]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
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
            var review = new Review
            {
                BarberId = model.BarberId,
                UserId = user.Id,
                Comment = model.Comment,
                Rating = model.Rating,
                CreatedOn = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review is successfully added!";
            return RedirectToAction("Details", "Barbers", new { id = model.BarberId, area = "" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Barber)
                .ThenInclude(b => b.User) 
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }


            var currentUserName = User.Identity?.Name;

            var isBarber = review.Barber?.User?.UserName == currentUserName;
            var isAdmin = User.IsInRole("Administrator");

            if (!isBarber && !isAdmin)
            {
                return Forbid();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Barbers", new { id = review.BarberId, area = "" });
        }

    }

}
