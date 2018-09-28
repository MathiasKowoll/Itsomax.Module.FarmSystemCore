
using System;
using System.Collections.Generic;

namespace Itsomax.Module.FarmSystemCore.ViewModels
{
    public class ConsumptionReport
    {
        public ConsumptionReport()
        {
            WarehouseOut = "07";
            Description = "CONSUMO";
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
        public long ConsumptionId { get; set; }

    }

    public class FolioConsumption
    {
        public long Folio { get; set; }
        public long ConsumptionId { get; set; }
    }

    public class ReportPreview
    {
        public ReportPreview()
        {
            GenerateReport = false;
        }
        public DateTime FromConsumptionDate { get; set; }
        public DateTime ToConsumptionDate { get; set; }
        public IList<ConsumptionReport> ReportTable { get; set; }
        public string WarehouseName { get; set; }
        public bool GenerateReport { get; set; }


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
        //public int Folio { get; set; }
        public string WarehouseName { get; set; }
    }
}
