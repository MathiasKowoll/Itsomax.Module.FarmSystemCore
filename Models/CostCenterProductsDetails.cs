namespace Itsomax.Module.FarmSystemCore.Models
{
    public class CostCenterProductsDetails
    {
        public CostCenterProductsDetails()
        {
            ProductOrder = 1;
        }
        public long ProductId { get; set; }
        public Products Product { get; set; }
        public long CostCenterProductsId { get;set; }
        public CostCenterProducts CostCenterProducts { get; set; }
        public long CostCenterId { get; set; }
        public CostCenter CostCenter { get; set; }
        public int ProductOrder { get; set; }
    }
}