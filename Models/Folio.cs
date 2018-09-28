using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models
{
    public class Folio : EntityBase
    {
        public bool IsPreviewFolio { get; set; }
        public DateTimeOffset InitialDate { get; set; }
        public DateTimeOffset FinalDate { get; set; }
        public IList<Consumptions> Consumptions { get; set; } = new List<Consumptions>();
        
    }
}
