using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Itsomax.Module.FarmSystemCore.ViewModels
{
    public class ProductCostCenterViewModel
    {
        [Required] public long CostCenterId {get; set; }
        [Required] public string Name { get; set; }
        [Required] public DateTimeOffset CreatedOn { get; set; }
        [Required] public bool Active { get; set; }
        public IEnumerable<SelectListItem> Products { get; set; }
    }
}