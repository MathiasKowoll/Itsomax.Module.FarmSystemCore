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

    public class ConsumptionViewModel
    {
        public string CostCenterName { get; set; }
        public long CostCenterId { get; set; }
        public IList<ProductList> ProductLists { get; set; } = new List<ProductList>();
    }

    public class ConsumptionSelectActivity
    {
        public long CostCenterId { get; set; }
    }
}