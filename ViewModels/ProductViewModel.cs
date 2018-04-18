using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.FarmSystemCore.ViewModels
{
    public class ProductViewModel
    {
        [Required]
        [MaxLength (200)]
        public string Code { get; set; }
        [Required]
        [MaxLength (100)]
        public string Name { get; set; }
        [MaxLength (500)]
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public long BaseUnitId { get; set; }
    }

    public class BaseUnitList
    {
        public long BaseUnitId { get; set; }
        public string BaseUnitName { get; set; }
        public bool Selected { get; set; }

    }

    public class ProductEditViewModel
    {
        [Required]
        public long Id { get; set; }
        [Required]
        [MaxLength (200)]
        public string Code { get; set; }
        [Required]
        [MaxLength (100)]
        public string Name { get; set; }
        [MaxLength (500)]
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public long BaseUnitId { get; set; }
    }
}