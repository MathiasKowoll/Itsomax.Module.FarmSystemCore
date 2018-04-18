using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models 
{
    public class BaseUnits : EntityBase 
    {
        public BaseUnits()
        {
            UpdatedOn = DateTimeOffset.Now;
        }
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
        [Required]
        public DateTimeOffset CreatedOn { get; set; }
        [Required]
        public DateTimeOffset UpdatedOn { get; set; }
        public IList<Products> Product { get; set; } = new List<Products>();
    }
}