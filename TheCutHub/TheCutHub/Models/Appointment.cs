using System.ComponentModel.DataAnnotations;


namespace TheCutHub.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BarberId { get; set; }
        public Barber Barber { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        public string? Notes { get; set; }
    }

}
