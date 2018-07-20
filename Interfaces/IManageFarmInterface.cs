using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Itsomax.Module.Core.Extensions;
using Itsomax.Module.FarmSystemCore.Models;
using Itsomax.Module.FarmSystemCore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Itsomax.Module.FarmSystemCore.Interfaces
{
    public interface IManageFarmInterface
    {
        Task<SystemSucceededTask> AddBaseUnit(BaseUnitViewModel model, string username);
        Task<SystemSucceededTask> EditBaseUnit(BaseUnitEditViewModel model, string userName);
        IEnumerable<BaseUnits> GetBaseUnitList();
        BaseUnits GetBaseUnitById(long id);
        BaseUnits GetBaseUnitByValue(string value);
        bool EnableDisableBaseUnit(long id, string username);
        Task<SystemSucceededTask> AddLocation(LocationViewModel model,string username);
        Task<SystemSucceededTask> EditLocation(LocationEditViewModel model, string username);
        bool EnableDisableLocation(long id, string username);
        IEnumerable<Locations> GetLocation();
        IEnumerable<Locations> GetActiveLocation();
        Locations GetLocationByName(string location);
        Locations GetLocationById(long id);
        List<LocationList> GetLocationList(long? costCenterId);
        Task<SystemSucceededTask> AddCostCenter(CostCenterViewModel model,string username);
        Task<SystemSucceededTask> EditCostCenter(CostCenterEditViewModel model, string username);
        bool EnableDisableCostCenter(long id, string username);
        IEnumerable<CostCenter> GetCostCenters();
        IEnumerable<CostCenter> GetActiveCostCenters();
        CostCenter GetCostCenterByName(string name);
        CostCenter GetCostCenterById(long id);
        Task<SystemSucceededTask> AddProduct(ProductViewModel model,string username);
        Task<SystemSucceededTask> EditProduct(ProductEditViewModel model, string username);
        bool EnableDisableProduct(long id, string username);
        IEnumerable<Products> GetProducts();
        IEnumerable<Products> GetActiveProducts();
        Products GetProductByName(string name);
        Products GetProductById(long id);
        Products GetProductByCode(string code);
        IEnumerable<BaseUnits> GetActiveBaseUnits();
        List<BaseUnitList> GetBaseUnitList(long? productId);
        Task<SystemSucceededTask> AddProductsToCostCenter(ProductCostCenterViewModel model, string username,params string[] selectedProducts);
        IEnumerable<SelectListItem> GetSelectListProducts(long centerCostId);
        string GetCostCenterProductName(long costCenterId);
        bool GetCostCenterProductActive(long costCenterId);
        IList<LocationList> GetCostCenterList();
        IList<LocationList> GetCostCenterMealList();
        IList<LocationList> GetCostCenterMedicallList();
        IEnumerable<ProductList> GetProductList(long costCenterId);
        IEnumerable<ProductListEdit> GetProductListEdit(long consumptionId);
        IEnumerable<ProductList> GetProductListFailed(long costCenterId, string[] keys, string[] values);
        IEnumerable<ProductListEdit> GetProductListEditFailed(long consumptionId, string[] keys, string[] values);
        Task<SystemSucceededTask> SaveConsumption(long costCenterId, string[] products, string[] values,string username,DateTimeOffset? lateCreatedOn);
        Task<SystemSucceededTask> SaveConsumptionEdit(long consumptionId, string[] products, string[] values,string username);
		Task<bool> LoadInitialDataFarm();
        IList<WarehouseList> GetWarehouseListNames();
        IList<ConsumptionReport> ConsumptionReport(DateTime reportDate, DateTime toReportDate, int folio,string warehouseName);
        IList<ConsumptionList> GetConsumptionList();
        Consumptions GetConsumptionById(long id);
    }
}