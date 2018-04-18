using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models
{
    public class ConsumptionTypes : EntityBase
    {
        [Required]
        public string Name { get; set; }
        public IList<Consumptions> Consumptions { get; set; } = new List<Consumptions>();
    }
}