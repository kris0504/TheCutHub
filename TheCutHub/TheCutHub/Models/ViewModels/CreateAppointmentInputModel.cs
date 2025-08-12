
using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models.ViewModels
{
    public class CreateAppointmentInputModel
    {
        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Choose service")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Choose barber")]
        public int BarberId { get; set; }

        [Required(ErrorMessage = "Choose timeslot")]
        public TimeSpan TimeSlot { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
