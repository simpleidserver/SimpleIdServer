using System.ComponentModel.DataAnnotations;

namespace UseUMAToProtectAPI.Portal.ViewModels
{
    public class AddPictureViewModel
    {
        [Required]
        public string Url { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
