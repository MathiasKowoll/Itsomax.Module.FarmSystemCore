using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models
{
    public class CostCenter : EntityBase
    {
        public CostCenter()
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
		[Required]
        public string WarehouseCode { get; set; }
		[Required]
		public bool IsMeal { get; set; }
		[Required]
		public bool IsMedical { get; set; }
		[Required]
		public bool IsFarming { get; set; }
        public long LocationId { get; set; }
        public Locations Locations { get; set; }
        public IList<ConsumptionDetails> ConsumptionDetails { get; set; } = new List<ConsumptionDetails>();
        public IList<CostCenterProductsDetails> CostCenterProductsDetails { get; set; } = new List<CostCenterProductsDetails>();
    }
}