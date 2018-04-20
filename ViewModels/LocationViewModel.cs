using System;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.FarmSystemCore.ViewModels
{
    public class LocationViewModel
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public DateTimeOffset CreatedOn { get; set; }
        [Required]
        public bool IsDefault { get; set; }
    }

    public class LocationEditViewModel
    {
        [Required]
        public long Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool IsDefault { get; set; }
    }
}