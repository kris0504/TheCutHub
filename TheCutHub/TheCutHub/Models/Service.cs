using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheCutHub.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        [Range(0, 500)]
        public decimal Price { get; set; }

        public int DurationMinutes { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
