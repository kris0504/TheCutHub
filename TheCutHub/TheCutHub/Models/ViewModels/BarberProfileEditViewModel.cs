using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models.ViewModels
{
    public class BarberProfileEditViewModel
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        [Display(Name = "Profile picture URL")]
        [Url(ErrorMessage = "Invalid URL.")]
        public string? ProfileImageUrl { get; set; }
        [Display(Name = "Upload new profile picture")]
        public IFormFile? ProfileImageFile { get; set; }
        [BindNever]
        public string? Email { get; set; }
        [BindNever]

        public ICollection<WorkImage> WorkImages { get; set; } = new List<WorkImage>();

    }
}
