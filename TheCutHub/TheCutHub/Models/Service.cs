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

        //[Required]
        //[Range(typeof(TimeSpan), "00:10:00", "04:00:00", ErrorMessage = "Duration must be between 10 minutes and 4 hours.")]
        //public TimeSpan DurationMinutes { get; set; }
        //[Required]
        //[Range(typeof(TimeSpan), "00:01:00", "23:59:59", ErrorMessage = "Duration must be between 1 minute and 24 hours.")]
        //[Column(TypeName = "time")]
        //public TimeSpan Duration { get; set; }
        public int DurationMinutes { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
