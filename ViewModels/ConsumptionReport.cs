
using System;

namespace Itsomax.Module.FarmSystemCore.ViewModels
{
    public class ConsumptionReport
    {
        public ConsumptionReport()
        {
            WarehouseOut = "07";
            Description = "CONSUMO SILAJE";
        }
        public string Warehouse { get; set; }
        public int Folio { get; set; }
        public string GeneratedDate { get; set; }
        public string WarehouseOut { get; set; }
        public string Description { get; set; }
        public string CenterCostCode { get; set; }
        public string ProductCode { get; set; }
        public string BaseUnit { get; set; }
        public decimal Amount { get; set; }

    }

    public class WarehouseList
    {
        public string WarehouseName { get; set; }
        public bool Selected { get; set; }
    }

    public class GenerateConsumptionReportViewModel
    {
        public DateTime FromConsumptionDate { get; set; }
        public DateTime ToConsumptionDate { get; set; }
        public int Folio { get; set; }
        public string WarehouseName { get; set; }
    }
}
