using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models
{
    public class Barber
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;           
        public ApplicationUser User { get; set; } = null!;    


        
        [MaxLength(100)]
        public string? FullName { get; set; }

        public string? ProfileImageUrl { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Review> Reviews { get; set; }
       

    }

}
