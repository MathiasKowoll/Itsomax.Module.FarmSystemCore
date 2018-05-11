using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models
{
    public class ProductTypes : EntityBase
    {
        [Required]
        public string Type { get; set; }
        public IList<Products> Product { get; set; } = new List<Products>();
    }
}