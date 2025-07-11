using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models
{
    public class Barber
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        public string? ProfileImageUrl { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
