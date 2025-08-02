using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Models.ViewModels;

namespace TheCutHub.Areas.Barber.Services
{
    public class BarberProfileService : IBarberProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BarberProfileService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<BarberProfileEditViewModel?> GetProfileAsync(string userId)
        {
            var barber = await _context.Barbers
                .Include(b => b.WorkImages)
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (barber == null) return null;

            return new BarberProfileEditViewModel
            {
                FullName = barber.FullName,
                Bio = barber.Bio,
                ProfileImageUrl = barber.ProfileImageUrl,
                WorkImages = barber.WorkImages
            };
        }

        public async Task<bool> UpdateProfileAsync(string userId, BarberProfileEditViewModel model)
        {
            var barber = await _context.Barbers.Include(b => b.WorkImages).FirstOrDefaultAsync(b => b.UserId == userId);
            if (barber == null) return false;

            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(barber.ProfileImageUrl) && !barber.ProfileImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, barber.ProfileImageUrl.TrimStart('/'));
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
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

            return true;
        }

        public async Task<bool> AddWorkImageAsync(string userId, AddWorkImageViewModel model)
        {
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == userId);
            if (barber == null || model.ImageFile == null) return false;

            if (model.ImageFile.ContentType != "image/jpeg" &&
                model.ImageFile.ContentType != "image/png" &&
                model.ImageFile.ContentType != "image/webp") return false;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
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

            _context.WorkImages.Add(workImage);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteWorkImageAsync(string userId, int imageId)
        {
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == userId);
            if (barber == null) return false;

            var image = await _context.WorkImages.FirstOrDefaultAsync(w => w.Id == imageId && w.BarberId == barber.Id);
            if (image == null) return false;

            _context.WorkImages.Remove(image);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
