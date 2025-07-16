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
        [Compare("StartTime", ErrorMessage = "End time must be after start time")]
        public TimeSpan EndTime { get; set; }

        public bool IsWorking { get; set; } = true;
    }
}
