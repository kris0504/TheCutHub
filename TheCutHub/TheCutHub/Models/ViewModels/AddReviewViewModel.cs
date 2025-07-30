using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models.ViewModels
{
    public class AddReviewViewModel
    {
        public int BarberId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Comment { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
