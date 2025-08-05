using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;
using TheCutHub.Services;
using X.PagedList;




namespace TheCutHub.Controllers
{
    [Authorize]
    
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ReviewsController(
            IReviewService reviewService,
            UserManager<ApplicationUser> userManager, 
            ApplicationDbContext context)
        {
            _reviewService = reviewService;
            _userManager = userManager;
            _context = context;
        }


        [AllowAnonymous]
        public IActionResult List(int barberId, int page = 1)
        {
            const int pageSize = 5;

            var reviewsQuery = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BarberId == barberId);

            var reviewCount = reviewsQuery.Count(); 
            
            var reviews = reviewsQuery
                .OrderByDescending(r => r.CreatedOn)
                .ToPagedList(page, pageSize);

            ViewBag.BarberId = barberId;

            return PartialView("_ReviewListPartial", reviews);
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


            var currentUserId = _userManager.GetUserId(User);
            var isAuthor = review.UserId == currentUserId;
            var isAdmin = User.IsInRole("Administrator");

            if (!isAuthor && !isAdmin)
                return Forbid();



            await _reviewService.DeleteAsync(review);

            return RedirectToAction("Details", "Barbers", new { id = review.BarberId, area = "" });
        }
    }
}
