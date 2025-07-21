using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models
{
    public class WorkingHour
    {

        public int Id { get; set; }

        [Required]
        public DayOfWeek Day { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        
        public TimeSpan EndTime { get; set; }

        public bool IsWorking { get; set; } = true;
        [Required]
        public int BarberId { get; set; }
        public Barber? Barber { get; set; }
    }
}
