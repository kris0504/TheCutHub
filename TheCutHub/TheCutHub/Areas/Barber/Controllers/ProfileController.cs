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
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        public ProfileController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context, ILogger<ProfileController> logger,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _env = env;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWorkImage(AddWorkImageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Form contains invalid data.";
                return View(model);
            }


            var user = await _userManager.GetUserAsync(User);
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == user.Id);

            if (barber == null || model.ImageFile == null)
                return BadRequest();


            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);


            var uniqueFileName = $"{Guid.NewGuid()}_{model.ImageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(fileStream);
            }


            var workImage = new WorkImage
            {
                BarberId = barber.Id,
                ImageUrl = "/uploads/" + uniqueFileName
            };
            if (model.ImageFile.ContentType != "image/jpeg" &&
                model.ImageFile.ContentType != "image/png" &&
                model.ImageFile.ContentType != "image/webp")
            {
                TempData["Fail"] = "Only JPG, PNG and WebP files are allowed.";
                return RedirectToAction("Edit");
            }

            _context.WorkImages.Add(workImage);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Image was successfully added!";
            return RedirectToAction("Edit");

        }



        // GET Barber/Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var barber = await _context.Barbers
                .Include(b => b.User)
                .Include(b => b.WorkImages)
                .FirstOrDefaultAsync(b => b.UserId == user.Id);

            if (barber == null) return NotFound();

            var model = new BarberProfileEditViewModel
            {
                FullName = barber.FullName,
                Bio = barber.Bio,
                ProfileImageUrl = barber.ProfileImageUrl,
                WorkImages = barber.WorkImages
            };
           

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkImage(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == user.Id);

            if (barber == null) return NotFound();

            var image = await _context.WorkImages
                .FirstOrDefaultAsync(w => w.Id == id && w.BarberId == barber.Id);

            if (image == null)
            {
                TempData["Fail"] = "Image not found.";
                return RedirectToAction("Edit");
            }

            _context.WorkImages.Remove(image);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Image successfully deleted.";
            return RedirectToAction("Edit");
        }

        // POST Barber/Profile/Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BarberProfileEditViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var barber = await _context.Barbers
                .Include(b => b.WorkImages)  
                .FirstOrDefaultAsync(b => b.UserId == user.Id);

            if (barber == null)
            {
                TempData["Fail"] = "Profile not found.";
                return RedirectToAction(nameof(Edit));
            }

            if (!ModelState.IsValid)
            {
               
                model.WorkImages = barber.WorkImages;
                return View(model);
            }
            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
               
                if (!string.IsNullOrEmpty(barber.ProfileImageUrl) &&
                    !barber.ProfileImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, barber.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}_{model.ProfileImageFile.FileName}";
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImageFile.CopyToAsync(stream);
                }

                barber.ProfileImageUrl = "/uploads/" + fileName;
            }

            barber.FullName = model.FullName;
            barber.Bio = model.Bio;

            _context.Barbers.Update(barber);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile successfully updated.";
            return RedirectToAction(nameof(Edit));
        }




    }

}
