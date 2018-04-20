using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models
{
    public class CostCenterProducts : EntityBase
    {
        public CostCenterProducts()
        {
            UpdatedOn = DateTimeOffset.Now;
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public DateTimeOffset CreatedOn { get; set; }
        [Required]
		public DateTimeOffset UpdatedOn { get; set; }
        public IList<CostCenterProductsDetails> CostCenterProductsDetails { get; set; } = new List<CostCenterProductsDetails>();
    }
}