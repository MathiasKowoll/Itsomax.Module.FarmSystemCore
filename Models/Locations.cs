using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models
{
    public class Locations : EntityBase 
    {
        public Locations()
        {
            UpdatedOn = DateTimeOffset.Now;
        }
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
        public DateTimeOffset UpdatedOn { get; set; }
        public IList<CostCenter> CostCenter { get; set; } = new List<CostCenter>();
    }
}