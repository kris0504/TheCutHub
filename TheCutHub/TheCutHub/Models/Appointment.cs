using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations.Schema;


namespace TheCutHub.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required]
        public TimeSpan TimeSlot { get; set; }

        [NotMapped]
        public DateTime AppointmentDateTime => Date.Date + TimeSlot;

        public int BarberId { get; set; }
        public Barber Barber { get; set; }
        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public string? Notes { get; set; }
    }

}
