using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}
