using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.FarmSystemCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Itsomax.Module.FarmSystemCore.Data
{
    public class FarmCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Products>(o =>
            {
                o.HasOne(ur => ur.BaseUnit).WithMany(x => x.Product).HasForeignKey(x => x.BaseUnitId);
                o.HasOne(ur => ur.ProductTypes).WithMany(x => x.Product).HasForeignKey(x => x.ProductTypeId).IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<ConsumptionDetails>(o =>
            {
                o.HasOne(x => x.Consumption).WithMany(x => x.ConsumptionDetails).HasForeignKey(x => x.ConsumptionId);
                o.HasOne(x => x.Product).WithMany(x => x.ConsumptionDetails).HasForeignKey(x => x.ProductId);
                o.HasOne(x => x.CostCenter).WithMany(x => x.ConsumptionDetails).HasForeignKey(x => x.CostCenterId);
                o.ToTable("ConsumptionDetails", "FarmCore");
            });
            modelBuilder.Entity<CostCenterProductsDetails>(o =>
            {
                o.HasOne(x => x.CostCenterProducts).WithMany(x => x.CostCenterProductsDetails)
                    .HasForeignKey(x => x.CostCenterProductsId);
                o.HasOne(x => x.Product).WithMany(x => x.CostCenterProductsDetails).HasForeignKey(x => x.ProductId);
                o.HasOne(x => x.CostCenter).WithMany(x => x.CostCenterProductsDetails)
                    .HasForeignKey(x => x.CostCenterId);
                o.ToTable("CostCenterProductsDetails", "FarmCore");

            });
            modelBuilder.Entity<CostCenter>(o =>
            {
                o.HasOne(x => x.Locations).WithMany(x => x.CostCenter).HasForeignKey(x => x.LocationId);
            });
        }
    }
}