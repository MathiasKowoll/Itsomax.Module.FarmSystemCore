using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models
{
    public class Consumptions : EntityBase
    {
        public Consumptions()
        {
            CreatedOn = DateTimeOffset.Now;
        }
        [Required]
        public string Name { get; set; }
        [Required]
		public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LateCreatedOn { get; set; }
        public IList<ConsumptionDetails> ConsumptionDetails { get; set; } = new List<ConsumptionDetails>();
    }
}