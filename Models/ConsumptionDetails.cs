using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models
{
    public class ConsumptionDetails : EntityBase
    {
        
        [Required]
        public long CostCenterId { get; set; }
        public CostCenter CostCenter { get; set; }
        [Required]
        public long ProductId { get; set; }
        public Products Product { get; set; }
        [Required]
        public long ConsumptionId { get; set; }
        public Consumptions Consumption { get; set; }
        [Required]
        public string BaseUnit { get; set; }
        [Required]
        public int Weight { get; set; }
        [Required]
        public string WarehouseCode { get; set; }
        
    }
}