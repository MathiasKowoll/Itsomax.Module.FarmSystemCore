using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models 
{
    public class Products : EntityBase 
    {
        public Products()
        {
            UpdatedOn = DateTime.Now;
        }
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
        public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset UpdatedOn { get; set; }
        [Required]
        public long BaseUnitId { get; set; }
        public BaseUnits BaseUnit { get; set; }
        public long? ProductTypeId { get; set; }
        public ProductTypes ProductTypes { get; set; }
        public IList<ConsumptionDetails> ConsumptionDetails { get; set; } = new List<ConsumptionDetails>();

        public IList<CostCenterProductsDetails> CostCenterProductsDetails { get; set; } =
            new List<CostCenterProductsDetails>();

    }
}