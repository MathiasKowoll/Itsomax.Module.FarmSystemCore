﻿using Itsomax.Data.Infrastructure.Models;

namespace Itsomax.Module.FarmSystemCore.Models
{
    public class CostCenterProductsDetails : EntityBase
    {
        public long ProductId { get; set; }
        public Products Product { get; set; }
        public long CostCenterProductsId { get;set; }
        public CostCenterProducts CostCenterProducts { get; set; }
        public long CostCenterId { get; set; }
        public CostCenter CostCenter { get; set; }
    }
}