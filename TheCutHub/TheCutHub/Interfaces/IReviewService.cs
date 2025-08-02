using System;
using System.Threading.Tasks;
using TheCutHub.Models;

namespace TheCutHub.Services
{
    public interface IReviewService
    {
        Task AddAsync(int barberId, string userId, string comment, int rating, DateTime createdOn);
        Task<Review?> GetByIdAsync(int id);
        Task DeleteAsync(Review review);
    }
}
