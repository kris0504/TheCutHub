using System.ComponentModel.DataAnnotations;

namespace TheCutHub.Models.ViewModels
{
    public class AddWorkImageViewModel
    {
        [Required]
       
        public IFormFile ImageFile { get; set; } = null!;
    }
}
