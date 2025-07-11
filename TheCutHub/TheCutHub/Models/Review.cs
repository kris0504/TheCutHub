using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models
{
    public class Review
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BarberId { get; set; }
        public Barber Barber { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

}
