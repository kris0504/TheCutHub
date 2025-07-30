using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models
{
    public class Review
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public int BarberId { get; set; }
        public Barber Barber { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
        [Required]
        [MaxLength(500)]
        public string? Comment { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

}
