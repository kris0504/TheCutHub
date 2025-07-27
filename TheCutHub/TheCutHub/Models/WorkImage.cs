using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models
{
    public class WorkImage
    {
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        public int BarberId { get; set; }
        public Barber Barber { get; set; } = null!;
    }
}
