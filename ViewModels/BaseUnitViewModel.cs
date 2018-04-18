using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.FarmSystemCore.ViewModels
{
    public class BaseUnitViewModel
    {
        [Required]
        [MaxLength (200)]
        public string Name { get; set; }
        [Required]
        [MaxLength (20)]
        public string Value { get; set; }
        [Required]
        [MaxLength (200)]
        public string Description { get; set; }
        [Required]
        public  bool Active { get; set; }
    }
}