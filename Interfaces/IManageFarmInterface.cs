using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Itsomax.Module.Core.ViewModels;
using Itsomax.Module.FarmSystemCore.Models;
using Itsomax.Module.FarmSystemCore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Itsomax.Module.FarmSystemCore.Interfaces
{
    public interface IManageFarmInterface
    {
        Task<SuccessErrorHandling> AddBaseUnit(BaseUnitViewModel model, string username);
        IEnumerable<BaseUnits> GetBaseUnitList();
        BaseUnits GetBaseUnitById(long id);
        BaseUnits GetBaseUnitByValue(string value);
        bool EnableDisableBaseUnit(long id, string username);
        Task<SuccessErrorHandling> AddLocation(LocationViewModel model,string username);
        Task<SuccessErrorHandling> EditLocation(LocationEditViewModel model, string username);
        bool EnableDisableLocation(long id, string username);
        IEnumerable<Locations> GetLocation();
        IEnumerable<Locations> GetActiveLocation();
        Locations GetLocationByName(string location);
        Locations GetLocationById(long id);
        List<LocationList> GetLocationList(long? costCenterId);
        Task<SuccessErrorHandling> AddCostCenter(CostCenterViewModel model,string username);
        Task<SuccessErrorHandling> EditCostCenter(CostCenterEditViewModel model, string username);
        bool EnableDisableCostCenter(long id, string username);
        IEnumerable<CostCenter> GetCostCenters();
        IEnumerable<CostCenter> GetActiveCostCenters();
        CostCenter GetCostCenterByName(string name);
        CostCenter GetCostCenterById(long id);
        Task<SuccessErrorHandling> AddProduct(ProductViewModel model,string username);
        Task<SuccessErrorHandling> EditProduct(ProductEditViewModel model, string username);
        bool EnableDisableProduct(long id, string username);
        IEnumerable<Products> GetProducts();
        IEnumerable<Products> GetActiveProducts();
        Products GetProductByName(string name);
        Products GetProductById(long id);
        Products GetProductByCode(string code);
        IEnumerable<BaseUnits> GetActiveBaseUnits();
        List<BaseUnitList> GetBaseUnitList(long? productId);
        Task<SuccessErrorHandling> AddProductsToCostCenter(ProductCostCenterViewModel model, string username,params string[] selectedProducts);
        IEnumerable<SelectListItem> GetSelectListProducts(long centerCostId);
        string GetCostCenterProductName(long costCenterId);
        bool GetCostCenterProductActive(long costCenterId);
        IList<LocationList> GetCostCenterList();
        IList<LocationList> GetCostCenterMealList();
        IEnumerable<ProductList> GetProductList(long costCenterId);
        Task<SuccessErrorHandling> SaveConsumption(long costCenterId, string[] products, string[] values,string username);
		Task<bool> LoadInitialDataFarm();
        IList<WarehouseList> GetWarehouseListNames();
        IEnumerable<ConsumptionReport> ConsumptionReport(DateTime reportDate, int folio,string warehouseName);
    }
}