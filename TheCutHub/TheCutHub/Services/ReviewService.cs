using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;

namespace TheCutHub.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(int barberId, string userId, string comment, int rating, DateTime createdOn)
        {
            var review = new Review
            {
                BarberId = barberId,
                UserId = userId,
                Comment = comment,
                Rating = rating,
                CreatedOn = createdOn
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Barber)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
    