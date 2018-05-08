using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Itsomax.Module.FarmSystemCore.ViewModels
{
    public class CostCenterViewModel
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
        public bool IsMeal { get; set; }
        [Required]
        public bool IsMedical { get; set; }
        [Required]
        public bool IsFarming { get; set; }
        public string WarehouseCode { get; set; }
        public long LocationId { get; set; }
    }

    public class LocationList
    {
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public bool Selected { get; set; }

    }

    public class CostCenterEditViewModel
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
        public bool IsMeal { get; set; }
        [Required]
        public bool IsMedical { get; set; }
        [Required]
        public bool IsFarming { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public long LocationId { get; set; }
        public string WarehouseCode { get; set; }
        public IEnumerable<SelectListItem> LocationList { get; set; }
    }
}