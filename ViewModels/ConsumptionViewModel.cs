using System;
using System.Collections.Generic;

namespace Itsomax.Module.FarmSystemCore.ViewModels
{
    public class ProductList
    {
        public string Name { get;set; }
        public string Value { get; set; }
        public string BaseUnit { get; set; }
        public string CenterCostName { get; set; }
    }
    public class ProductListEdit
    {
        public long CostCenterId { get; set; }
        public string Name { get;set; }
        public decimal Value { get; set; }
        public string BaseUnit { get; set; }
        public string CenterCostName { get; set; }
    }

    public class ConsumptionViewModel
    {
        public string CostCenterName { get; set; }
        public long CostCenterId { get; set; }
        public IList<ProductList> ProductLists { get; set; } = new List<ProductList>();
        public DateTimeOffset LateDateTime { get; set; }
    }

    public class ConsumptionEditViewModel
    {
        public string CostCenterName { get; set; }
        public long ConsumptionId { get; set; }
        public IList<ProductListEdit> ProductListEdit { get; set; } = new List<ProductListEdit>();
        public DateTimeOffset LateDateTime { get; set; }
    }

    public class ConsumptionSelectActivity
    {
        public long CostCenterId { get; set; }
    }

    public class ConsumptionList
    {
        public long Id { get; set; }
        public string ConsumptionName { get; set; }
        public string Warehouse { get; set; }
        public string CenterCost { get; set; }
        public string ConsumptionEffectiveEntryDate { get; set; }
        public string ConsumptionEntryDate { get; set; }
    }
}