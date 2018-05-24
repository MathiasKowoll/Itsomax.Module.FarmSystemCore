using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Extensions;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.Core.ViewModels;
using Itsomax.Module.FarmSystemCore.Interfaces;
using Itsomax.Module.FarmSystemCore.Models;
using Itsomax.Module.FarmSystemCore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Itsomax.Module.FarmSystemCore.Services
{
    public class ManageFarmInterface : IManageFarmInterface
    {
        private readonly IRepository<Locations> _location;
        private readonly IRepository<CostCenter> _costCenter;
        private readonly IRepository<Products> _products;
        private readonly IRepository<CostCenterProductsDetails> _costCenterProductDetail;
        private readonly IRepository<CostCenterProducts> _costCenterProduct;
        private readonly IRepository<BaseUnits> _baseUnits;
        private readonly IRepository<Consumptions> _consumption;
        private readonly IRepository<ConsumptionDetails> _consumptioDetails;
        private readonly ILogginToDatabase _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        
        public ManageFarmInterface(IRepository<Locations> location,ILogginToDatabase logger,IRepository<CostCenter> costCenter,
            IRepository<Products> products,IRepository<CostCenterProductsDetails> costCenterProductDetail,
            IRepository<CostCenterProducts> costCenterProduct,IRepository<BaseUnits> baseUnits,IRepository<Consumptions> consumption,
            IRepository<ConsumptionDetails> consumptioDetails, IHostingEnvironment hostingEnvironment)
        {
            _location = location;
            _logger = logger;
            _costCenter = costCenter;
            _products = products;
            _costCenterProductDetail = costCenterProductDetail;
            _costCenterProduct = costCenterProduct;
            _baseUnits = baseUnits;
            _consumption = consumption;
            _consumptioDetails = consumptioDetails;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<SuccessErrorHandling> AddBaseUnit(BaseUnitViewModel model, string username)
        {
            var baseUnit = new BaseUnits
            {
                CreatedOn = DateTimeOffset.Now,
                Active = model.Active,
                Description = model.Description,
                Name = model.Name,
                Value = model.Value
            };
            _baseUnits.Add(baseUnit);
            try
            {
                await _baseUnits.SaveChangesAsync();
                var success = _logger.SuccessErrorHandlingTask("Base unit " + model.Name + " created successfully",
                    "Success", "Base unit " + model.Name + " created successfully", true);
                _logger.ErrorLog(success.LoggerMessage, "Create Base Unit", String.Empty, username);
                return success;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Base unit " + model.Name + " created unsuccessfully",
                    "Error", "Base unit " + model.Name + " created unsuccessfully", false);
                _logger.ErrorLog(error.LoggerMessage, "Create Base Unit", ex.InnerException.Message, username);
                return error;
            }
        }

        public IEnumerable<BaseUnits> GetBaseUnitList()
        {
            return _baseUnits.Query().ToList();
        }

        public BaseUnits GetBaseUnitById(long id)
        {
            return _baseUnits.Query().FirstOrDefault(x => x.Id == id);
        }
        public BaseUnits GetBaseUnitByValue(string value)
        {
            return _baseUnits.Query().FirstOrDefault(x => x.Value == value);
        }

        public async Task<SuccessErrorHandling> AddLocation(LocationViewModel model,string username)
        {
            if (!ValidateLocationCode(model.Code))
            {
                var error = _logger.SuccessErrorHandlingTask("Location " + model.Name + " code already exists","Error","Location " + model.Name + " code already exists",false);
                _logger.ErrorLog(error.LoggerMessage,"Create Location","Code for location already exists",username);
                return error;
            }

            var location = new Locations()
            {
                Active = model.Active,
                Code = model.Code,
                CreatedOn = DateTimeOffset.Now,
                Description = model.Description,
                Name = model.Name,
            };
            _location.Add(location);
            
            try
            {
                
                await _location.SaveChangesAsync();
                var success = _logger.SuccessErrorHandlingTask("Location " + model.Name + " created successfully","Success","Location " + model.Name + " created successfully",true);
                _logger.ErrorLog(success.LoggerMessage,"Create Location",String.Empty,username);
                return success;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Location " + model.Name + " created unsuccessfully","Error","Location " + model.Name + " created unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Create Location",ex.InnerException.Message,username);
                return error;
            }
        }

        public async Task<SuccessErrorHandling> EditLocation(LocationEditViewModel model, string username)
        {
            if (model == null)
            {
                var error = _logger.SuccessErrorHandlingTask("Location not found","Error","Location  not found",false);
                _logger.ErrorLog(error.LoggerMessage,"Edit Location ","Location not found",username);
                return error;
            }

            var location = await _location.Query().FirstOrDefaultAsync(x => x.Id == model.Id);
            location.Active = model.Active;
            location.CreatedOn = location.CreatedOn;
            location.Name = model.Name;
            location.Description = model.Description;

            try
            {
                await _location.SaveChangesAsync();
                var success = _logger.SuccessErrorHandlingTask("Location " + model.Name + " updated successfully","Success","Location " + model.Name + " updated successfully",true);
                _logger.ErrorLog(success.LoggerMessage,"Update Location",String.Empty,username);
                return success;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Location " + model.Name + " updated unsuccessfully","Error","Location " + model.Name + " updated unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Update Location",ex.InnerException.Message,username);
                return error;
            }
        }
        

        public bool EnableDisableLocation(long id,string username)
        {

            var locations = _location.Query().FirstOrDefaultAsync(x => x.Id == id).Result;
            if (locations == null)
                return false;

            switch (locations.Active)
            {
                case true:
                    locations.Active = false;
                    break;
                case false:
                    locations.Active = true;
                    break;
            }

            try
            {
                _location.SaveChanges();
                return true;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Location " + locations.Name + " Enable/Disable unsuccessfully","Error","Location " + locations.Name + " Enable/Disable unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Enable/Disable Location",ex.InnerException.Message,username);
                return false;
            }
        }

        private bool ValidateLocationCode(string code)
        {
            var exists = _location.Query().FirstOrDefault(x => x.Code == code);
            return exists == null;
        }

        public IEnumerable<Locations> GetLocation()
        {
            return _location.Query();
        }
        public IEnumerable<Locations> GetActiveLocation()
        {
            return _location.Query().Where(x => x.Active);
        }

        public Locations GetLocationByName(string location)
        {
            return _location.Query().FirstOrDefault(x => x.Name == location);
        }

        public Locations GetLocationById(long id)
        {
            return _location.Query().FirstOrDefault(x => x.Id == id);
        }

        public List<LocationList> GetLocationList(long? costCenterId)
        {
            if (costCenterId == null)
            {
                var locationList = new List<LocationList>();   
                var locList = (from a in GetActiveLocation()
                    select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
                locationList.Add( new LocationList {LocationId = 0, LocationName = "Select a Location",Selected = true});
                foreach (var item in locList)
                {
                    locationList.Add( new LocationList {LocationId = item.LocationId,LocationName = item.LocationName,Selected = false});
                }

                return locationList;
            }
            else
            {
                var costCenter = GetCostCenterById(costCenterId.Value);

                var locationList = new List<LocationList>();
                var locList = (from a in GetActiveLocation()
                    select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
                foreach (var item in locList)
                {
                    bool selected = item.LocationId == costCenter.LocationId;
                    locationList.Add( new LocationList {LocationId = item.LocationId,LocationName = item.LocationName,Selected = selected});
                    
                }
                return locationList;

            }
        }        
        

        public async Task<SuccessErrorHandling> AddCostCenter(CostCenterViewModel model,string username)
        {
            if (!ValidateCostCenterCode(model.Code))
            {
                var error = _logger.SuccessErrorHandlingTask("Cost center " + model.Name + " code already exists","Error","Cost Center " + model.Name + " code already exists",false);
                _logger.ErrorLog(error.LoggerMessage,"Create Cost Center","Code for cost center already exists",username);
                return error;
            }

            var centerCost = new CostCenter()
            {
                Active = model.Active,
                Code = model.Code,
                CreatedOn = DateTimeOffset.Now,
                Description = model.Description,
                WarehouseCode = model.WarehouseCode,
                Name = model.Name,
                LocationId = model.LocationId,
                IsFarming = model.IsFarming,
                IsMeal = model.IsMeal,
                IsMedical = model.IsMedical
            };
            _costCenter.Add(centerCost);

            try
            {
                
                await _costCenter.SaveChangesAsync();
                var success = _logger.SuccessErrorHandlingTask("Cost Center " + model.Name + " created successfully","Success","Cost Center " + model.Name + " created successfully",true);
                _logger.InformationLog(success.LoggerMessage,"Create Cost Center",String.Empty,username);

                return success;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Cost Center " + model.Name + " created unsuccessfully","Error","Cost Center " + model.Name + " created unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Create Cost Center",ex.InnerException.Message,username);
                return error;
            }
        }

        public async Task<SuccessErrorHandling> EditCostCenter(CostCenterEditViewModel model,string username)
        {
            if (model == null)
            {
                var error = _logger.SuccessErrorHandlingTask("Cost Center not found","Error","Cost Center  not found",false);
                _logger.ErrorLog(error.LoggerMessage,"Edit Cost Center","Cost Center not found",username);
                return error;
            }

            var costCenter = await _costCenter.Query().FirstAsync(x => x.Id == model.Id);
            costCenter.Active = model.Active;
            costCenter.LocationId = model.LocationId;
            costCenter.CreatedOn = costCenter.CreatedOn;
            costCenter.Name = model.Name;
            costCenter.Description = model.Description;
            costCenter.IsFarming = model.IsFarming;
            costCenter.IsMeal = model.IsMeal;
            costCenter.IsMedical = model.IsMedical;
            costCenter.WarehouseCode = model.WarehouseCode;
            try
            {
                await _costCenter.SaveChangesAsync();
                var success = _logger.SuccessErrorHandlingTask("Cost Center " + model.Name + " updated successfully","Success","Cost Center " + model.Name + " updated successfully",true);
                _logger.InformationLog(success.LoggerMessage,"Edit Cost Center",String.Empty,username);
                return success;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Edit Center " + model.Name + " updated unsuccessfully","Error","Cost Center " + model.Name + " updated unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Edit Cost Center",ex.InnerException.Message,username);
                return error;
            }
        }

        public bool EnableDisableCostCenter(long id,string username)
        {

            var costCenter = _costCenter.Query().FirstOrDefaultAsync(x => x.Id == id).Result;
            if (costCenter == null)
                return false;

            switch (costCenter.Active)
            {
                case true:
                    costCenter.Active = false;
                    break;
                case false:
                    costCenter.Active = true;
                    break;
            }

            try
            {
                _location.SaveChanges();
                return true;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Cost Center " + costCenter.Name + " Enable/Disable unsuccessfully","Error","Product " + costCenter.Name + " Enable/Disable unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Enable/Disable Cost Center",ex.InnerException.Message,username);
                return false;
            }
        }

        private bool ValidateCostCenterCode(string code)
        {
            var exists = _costCenter.Query().FirstOrDefault(x => x.Code == code);
            return exists == null;
        }
        
        public async Task<SuccessErrorHandling> AddProductsToCostCenter(ProductCostCenterViewModel model,string username,params string[] selectedProducts)
        {
            //TODO: improve try catch
            var productsCostCenter = _costCenterProductDetail.Query().Where(x => x.CostCenterId == model.CostCenterId).ToList();
            if (productsCostCenter.Any())
            {
                foreach (var item in productsCostCenter)
                {
                    _costCenterProductDetail.Remove(_costCenterProductDetail.Query().FirstOrDefault(x => x.CostCenterId == model.CostCenterId && x.ProductId == item.ProductId));
                }
                await _costCenterProductDetail.SaveChangesAsync();
            }

            var costCenterProduct = new CostCenterProducts
            {
                Name = model.Name,
                CreatedOn = DateTimeOffset.Now,
                Active = model.Active
            };
            _costCenterProduct.Add(costCenterProduct);
            await _costCenterProduct.SaveChangesAsync();

            foreach (var item in selectedProducts)
            {
                var product = GetProductByName(item);
                var costCenterDetail = new CostCenterProductsDetails
                {
                    CostCenterId = model.CostCenterId,
                    ProductId = product.Id,
                    CostCenterProductsId = costCenterProduct.Id
                };
                _costCenterProductDetail.Add(costCenterDetail);
            }

            try
            {
                await _costCenterProductDetail.SaveChangesAsync();
                var success = _logger.SuccessErrorHandlingTask("Cost Center " + model.Name + " : product added successfully","Success","Cost Center " + model.Name + " : product added successfully",true);
                _logger.InformationLog(success.LoggerMessage,"Edit Cost Center",String.Empty,username);
                return success;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Cost Center " + model.Name + " product added unsuccessfully","Error","Cost Center " + model.Name + " product added unsuccessfully",false);
                _logger.InformationLog(error.LoggerMessage,"Edit Cost Center",ex.InnerException.Message,username);
                return error;
            }
        }

        public string GetCostCenterProductName(long costCenterId)
        {
            var name = (from cc in _costCenter.Query()
                join pd in _costCenterProductDetail.Query() on cc.Id equals pd.CostCenterId
                join ccname in _costCenterProduct.Query() on pd.CostCenterProductsId equals ccname.Id
                where cc.Id == costCenterId
                select(ccname.Name)).FirstOrDefault();
            return name;
        }
        
        public bool GetCostCenterProductActive(long costCenterId)
        {
            var active = (from cc in _costCenter.Query()
                join pd in _costCenterProductDetail.Query() on cc.Id equals pd.CostCenterId
                join ccname in _costCenterProduct.Query() on pd.CostCenterProductsId equals ccname.Id
                where cc.Id == costCenterId
                select(ccname.Active)).FirstOrDefault();
            return active;
        }

        public IEnumerable<SelectListItem> GetSelectListProducts(long centerCostId)
        {
            return _products.Query().Select(x => new SelectListItem
            {
                Selected = GetProductsForCostCenter(centerCostId).Contains(x.Name),
                Text = x.Name,
                Value = x.Name
            });
        }

        public IList<string> GetProductsForCostCenter(long centerCostId)
        {
            var products = from cc in _costCenter.Query().ToList()
                join pd in _costCenterProductDetail.Query().ToList() on cc.Id equals pd.CostCenterId
                join pr in _products.Query().ToList() on pd.ProductId equals pr.Id
                where cc.Id == centerCostId
                select(pr.Name);
            return products.ToList();
        }

        public async Task<SuccessErrorHandling> SaveConsumption(long costCenterId, string[] products, string[] values,string username,DateTimeOffset? lateCreatedOn)
        {
            var count = products.Length;
            var costCenter = GetCostCenterById(costCenterId);
            var consumptionHeader = new Consumptions
            {
				Name = "Added meal in " + costCenter.Name + " " + DateTimeOffset.Now,
                LateCreatedOn = lateCreatedOn ?? DateTimeOffset.Now
                
                
            };
            _consumption.Add(consumptionHeader);

            try
            {
                await _consumption.SaveChangesAsync();
                for (var i = 0; i < count;  i++)
                {
                    var getProduct = await (from a in _products.Query()
                        join b in _baseUnits.Query() on a.BaseUnitId equals b.Id
                        where a.Name == products[i]
                        select new {a.Id, a.Name, BaseUnit = b.Value}).FirstOrDefaultAsync();
                    var consumptionDetail = new ConsumptionDetails
                    {
                        ConsumptionId = consumptionHeader.Id,
                        CostCenterId = costCenter.Id,
                        WarehouseCode = costCenter.WarehouseCode,
                        ProductId = getProduct.Id,
                        BaseUnit = getProduct.BaseUnit,
                        Weight = Convert.ToInt32(values[i])
                        
                    };
                    _consumptioDetails.Add(consumptionDetail);
                }
                try
                {
                    await _consumptioDetails.SaveChangesAsync();
                    var success = _logger.SuccessErrorHandlingTask("Consumption " + consumptionHeader.Name + " : created successfully","Success","Consumption " + consumptionHeader.Name + " : created successfully",true);
                    _logger.InformationLog(success.LoggerMessage,"Create Consumption",String.Empty,username);
                    return success;
                }
                catch (SqlException ex)
                {
                    _consumption.Remove(consumptionHeader);
                    await _consumption.SaveChangesAsync();
                    var error = _logger.SuccessErrorHandlingTask("Consumption " + consumptionHeader.Name + " created unsuccessfully","Error","Consumption " + consumptionHeader.Name + " created unsuccessfully",false);
                    _logger.InformationLog(error.LoggerMessage,"Create Consumption Details",ex.InnerException.Message,username);
                    return error;
                }
                
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Consumption " + consumptionHeader.Name + " created unsuccessfully","Error","Consumption " + consumptionHeader.Name + " created unsuccessfully",false);
                _logger.InformationLog(error.LoggerMessage,"Create Consumption",ex.InnerException.Message,username);
                return error;
            }
        }

        public async Task<SuccessErrorHandling> SaveConsumptionEdit(long consumptionId, string[] products, string[] values,string username)
        {
            var count = products.Length;
            var consumptionHeader = _consumption.Query().FirstOrDefault(x => x.Id == consumptionId);
            var oldConsumptionDetail = _consumptioDetails.Query().Where(x => x.ConsumptionId == consumptionId).ToList();
            var oldConsumptionDate = consumptionHeader.CreatedOn;
            consumptionHeader.CreatedOn = DateTimeOffset.Now;

            foreach (var item in oldConsumptionDetail)
            {
                _consumptioDetails.Remove(_consumptioDetails.Query().FirstOrDefault(x => x.Id == item.Id));
            }

            
            try
            {
                await _consumption.SaveChangesAsync();
                await _consumptioDetails.SaveChangesAsync();
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        var getProduct = await (from a in _products.Query()
                            join b in _baseUnits.Query() on a.BaseUnitId equals b.Id
                            where a.Name == products[i]
                            select new {a.Id, a.Name, BaseUnit = b.Value}).FirstOrDefaultAsync();
                        var consumptionDetail = new ConsumptionDetails
                        {
                            ConsumptionId = consumptionHeader.Id,
                            CostCenterId = oldConsumptionDetail.FirstOrDefault().CostCenterId,
                            WarehouseCode = oldConsumptionDetail.FirstOrDefault().WarehouseCode,
                            ProductId = getProduct.Id,
                            BaseUnit = getProduct.BaseUnit,
                            Weight = Convert.ToInt32(values[i])
                        
                        };
                        _consumptioDetails.Add(consumptionDetail);
                    }

                    await _consumptioDetails.SaveChangesAsync();
                    var success = _logger.SuccessErrorHandlingTask("Consumption " + consumptionHeader.Name + " : updated successfully","Success","Consumption " + consumptionHeader.Name + " : updated successfully",true);
                    _logger.InformationLog(success.LoggerMessage,"Create Consumption",String.Empty,username);
                    return success;
                }
                catch (SqlException ex)
                {
                    var consumptionReverse = _consumption.Query().FirstOrDefault(x => x.Id == consumptionId);
                    consumptionReverse.CreatedOn = oldConsumptionDate;
                    await _consumption.SaveChangesAsync();
                    var error = _logger.SuccessErrorHandlingTask("Consumption " + consumptionHeader.Name + " updated unsuccessfully","Error","Consumption " + consumptionHeader.Name + " updated unsuccessfully",false);
                    _logger.InformationLog(error.LoggerMessage,"Create Consumption",ex.InnerException.Message,username);
                    return error;
                }
                
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Consumption " + consumptionHeader.Name + " updated unsuccessfully","Error","Consumption " + consumptionHeader.Name + " updated unsuccessfully",false);
                _logger.InformationLog(error.LoggerMessage,"Create Consumption",ex.InnerException.Message,username);
                return error;
            }
        }
        

        public IEnumerable<CostCenter> GetCostCenters()
        {
            return _costCenter.Query();
        }

        public IEnumerable<CostCenter> GetActiveCostCenters()
        {
            return _costCenter.Query().Where(x => x.Active);
        }

        public CostCenter GetCostCenterByName(string name)
        {
            return _costCenter.Query().FirstOrDefault(x => x.Name == name);
        }

        public CostCenter GetCostCenterById(long id)
        {
            return _costCenter.Query().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<BaseUnits> GetActiveBaseUnits()
        {
            return _baseUnits.Query().Where(x => x.Active == true);
        }

        public List<BaseUnitList> GetBaseUnitList(long? productId)
        {
            if (productId == null)
            {
                var baseUnitLists = new List<BaseUnitList>();   
                var locList = (from a in GetActiveBaseUnits()
                    select new { BaseUnutId = a.Id, BaseUnitName = a.Name,Selected = false }).ToList();
                baseUnitLists.Add( new BaseUnitList {BaseUnitId = 0, BaseUnitName = "Select a Base Unit",Selected = true});
                foreach (var item in locList)
                {
                    baseUnitLists.Add( new BaseUnitList {BaseUnitId = item.BaseUnutId,BaseUnitName = item.BaseUnitName,Selected = false});
                }

                return baseUnitLists;
            }
            else
            {
                var product = GetProductById(productId.Value);

                var baseUnitLists = new List<BaseUnitList>();
                var locList = (from a in GetActiveBaseUnits()
                    select new { BaseUnitId = a.Id, BaseUnitName = a.Name,Selected = false }).ToList();
                foreach (var item in locList)
                {
                    bool selected = item.BaseUnitId == product.BaseUnitId;
                    baseUnitLists.Add( new BaseUnitList {BaseUnitId = item.BaseUnitId,BaseUnitName = item.BaseUnitName,Selected = selected});
                    
                }
                return baseUnitLists;

            }
        }

        public async Task<SuccessErrorHandling> AddProduct(ProductViewModel model,string username)
        {
            if (!ValidateProductCode(model.Code))
            {
                var error = _logger.SuccessErrorHandlingTask("Product " + model.Name + " code already exists","Error","Product " + model.Name + " code already exists",false);
                _logger.ErrorLog(error.LoggerMessage,"Create Product","Code for product already exists",username);
                return error;
            }
            var product = new Products
            {
                Active = model.Active,
                BaseUnitId = model.BaseUnitId,
                Code = model.Code,
                Description = model.Description,
                Name = model.Name
            };
            _products.Add(product);
            try
            {
                await _products.SaveChangesAsync();
                var success = _logger.SuccessErrorHandlingTask("Product " + model.Name + " created successfully","Success","Product " + model.Name + " created successfully",true);
                _logger.InformationLog(success.LoggerMessage,"Create Product",String.Empty);
                return success;

            }
            catch (Exception ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Product " + model.Name + " created unsuccessfully","Error","Product " + model.Name + " created unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Create Product",ex.InnerException.Message);
                return error;
            }
        }

        public async Task<SuccessErrorHandling> EditProduct(ProductEditViewModel model, string username)
        {
            if (model == null)
            {
                var error = _logger.SuccessErrorHandlingTask("Product not found","Error","Product not found",false);
                _logger.ErrorLog(error.LoggerMessage,"Edit Product","Product not found",username);
                return error;
            }

            var products = await _products.Query().FirstAsync(x => x.Id == model.Id);
            products.Active = model.Active;
            products.BaseUnitId = model.BaseUnitId;
            products.CreatedOn = products.CreatedOn;
            products.Name = model.Name;
            products.Description = model.Description;
            try
            {
                await _products.SaveChangesAsync();
                var success = _logger.SuccessErrorHandlingTask("Product " + model.Name + " updated successfully","Success","Product " + model.Name + " updated successfully",true);
                _logger.InformationLog(success.LoggerMessage,"Edit Product",String.Empty,username);
                return success;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Edit Product " + model.Name + " updated unsuccessfully","Error","Product " + model.Name + " updated unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Edit Product",ex.InnerException.Message,username);
                return error;
            }
        }

        public  bool EnableDisableProduct(long id,string username)
        {

            var products = _products.Query().FirstOrDefaultAsync(x => x.Id == id).Result;
            if (products == null)
                return false;

            switch (products.Active)
            {
                case true:
                    products.Active = false;
                    break;
                case false:
                    products.Active = true;
                    break;
            }

            try
            {
                _location.SaveChanges();
                return true;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Product " + products.Name + " Enable/Disable unsuccessfully","Error","Product " + products.Name + " Enable/Disable unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Enable/Disable Product",ex.InnerException.Message,username);
                return false;
            }
        }

        public bool EnableDisableBaseUnit(long id,string username)
        {

            var baseUnit = _baseUnits.Query().FirstOrDefaultAsync(x => x.Id == id).Result;
            if (baseUnit == null)
                return false;

            switch (baseUnit.Active)
            {
                case true:
                    baseUnit.Active = false;
                    break;
                case false:
                    baseUnit.Active = true;
                    break;
            }

            try
            {
                _location.SaveChanges();
                var success = _logger.SuccessErrorHandlingTask("Base Unit " + baseUnit.Name + " Enable/Disable successfully", "Success", "Base Unit " + baseUnit.Name + " Enable/Disable successfully", true);
                _logger.ErrorLog(success.LoggerMessage, "Enable/Disable Base Unit", String.Empty, username);
                return true;
            }
            catch (SqlException ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Base Unit " + baseUnit.Name + " Enable/Disable unsuccessfully","Error","Base Unit " + baseUnit.Name + " Enable/Disable unsuccessfully",false);
                _logger.ErrorLog(error.LoggerMessage,"Enable/Disable Base Unit",ex.InnerException.Message,username);
                return false;
            }
        }

        private bool ValidateProductCode(string code)
        {
            var exists = _products.Query().FirstOrDefault(x => x.Code == code);
            return exists == null;
        }

        public async Task<SucceededTask> AddActivityProducts(LocationViewModel model)
        {
            var activityProduct = new CostCenterProducts()
            {
                CreatedOn = DateTimeOffset.Now,
                Name = model.Name,
            };

            _costCenterProduct.Add(activityProduct);

            try
            {
                await _costCenterProduct.SaveChangesAsync();
                return SucceededTask.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return SucceededTask.Failed("");
            }
        }

        public IEnumerable<Products> GetProducts()
        {
            return _products.Query();
        }

        public IEnumerable<Products> GetActiveProducts()
        {
            return _products.Query().Where(x => x.Active);
        }

        public Products GetProductByName(string name)
        {
            return _products.Query().FirstOrDefault(x => x.Name == name);
        }

        public Products GetProductByCode(string code)
        {
            return _products.Query().FirstOrDefault(x => x.Code == code);
        }
        public Products GetProductById(long id)
        {
            return _products.Query().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<ProductList> GetProductList(long costCenterId)
        {
            var list = from cc in _costCenter.Query()
                join pd in _costCenterProductDetail.Query() on cc.Id equals pd.CostCenterId
                join pr in _products.Query() on pd.ProductId equals pr.Id
                join ccname in _costCenterProduct.Query() on pd.CostCenterProductsId equals ccname.Id
                join bu in _baseUnits.Query() on pr.BaseUnitId equals bu.Id
                where cc.Id == costCenterId && cc.Active == true && pr.Active == true
                select new ProductList
                {
                    Name = pr.Name,
                    BaseUnit = bu.Value,
                    Value = String.Empty,
                    CenterCostName = cc.Name
                };

            return list.ToList();
        }

        public IEnumerable<ProductListEdit> GetProductListEdit(long consumptionId)
        {
            var list = from cd in _consumptioDetails.Query().Where(x => x.ConsumptionId == consumptionId)
                join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
                join p in _products.Query() on cd.ProductId equals p.Id
                select new ProductListEdit
                {
                    CostCenterId = cc.Id,
                    CenterCostName = cc.Name,
                    Name = p.Name,
                    Value = cd.Weight,
                    BaseUnit = cd.BaseUnit

                };
            return list.ToList();
        }
        

        public IList<LocationList> GetCostCenterList()
        {
            var locationList = new List<LocationList>();   
            var locList = (from a in GetActiveCostCenters()
                select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
            locationList.Add( new LocationList {LocationId = 0, LocationName = "Select a Cost Center",Selected = true});
            foreach (var item in locList)
            {
                locationList.Add( new LocationList {LocationId = item.LocationId,LocationName = item.LocationName,Selected = false});
            }

            return locationList;
        }
        
        public IList<LocationList> GetCostCenterMealList()
        {
            var locationList = new List<LocationList>();   
            var locList = (from a in GetActiveCostCenters()
                where a.IsMeal == true
                select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
            locationList.Add( new LocationList {LocationId = 0, LocationName = "Select a Cost Center",Selected = true});
            foreach (var item in locList)
            {
                locationList.Add( new LocationList {LocationId = item.LocationId,LocationName = item.LocationName,Selected = false});
            }

            return locationList;
        }
        
        public IList<LocationList> GetCostCenterMedicallList()
        {
            var locationList = new List<LocationList>();   
            var locList = (from a in GetActiveCostCenters()
                where a.IsMedical == true
                select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
            locationList.Add( new LocationList {LocationId = 0, LocationName = "Select a Cost Center",Selected = true});
            foreach (var item in locList)
            {
                locationList.Add( new LocationList {LocationId = item.LocationId,LocationName = item.LocationName,Selected = false});
            }

            return locationList;
        }
        
        
        public async Task<bool> LoadInitialDataFarm()
        {
            //FarmInitialData
            var rootFolder = _hostingEnvironment.WebRootPath;
            var fileName = @"FarmInitial.xlsx";
            var path = Path.Combine(rootFolder, "FarmInitialData", fileName);
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new XSSFWorkbook(fs);
                    //Load Locations
                    var sheet = workbook.GetSheetAt(workbook.GetSheetIndex("Location"));
                    var headerRow = sheet.GetRow(0);
                    var cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum +1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var location = new Locations();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            
                            if (headerRow.GetCell(j).ToString() == "Name") location.Name = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Code") location.Code = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Description") location.Description = row.GetCell(j).ToString();
                            
                        }

                        location.Active = true;
                        location.CreatedOn = DateTimeOffset.Now;
                        _location.Add(location);
                        await _location.SaveChangesAsync();
                    }
                    //Load Cost Center
                    sheet = workbook.GetSheetAt(workbook.GetSheetIndex("CostCenter"));
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var costCenter = new CostCenter();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (headerRow.GetCell(j).ToString() == "Name") costCenter.Name = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Code") costCenter.Code = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Description") costCenter.Description = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "WarehouseCode") costCenter.WarehouseCode = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Location")costCenter.LocationId = GetLocationByName(row.GetCell(j).ToString()).Id;
                            if (headerRow.GetCell(j).ToString() == "Meal")costCenter.IsMeal = row.GetCell(j).ToString() == "1";
                            if (headerRow.GetCell(j).ToString() == "Medical")costCenter.IsMedical = row.GetCell(j).ToString() == "1";
                            if (headerRow.GetCell(j).ToString() == "Farm")costCenter.IsFarming = row.GetCell(j).ToString() == "1";
                            

                        }

                        costCenter.Active = true;
                        costCenter.CreatedOn = DateTimeOffset.Now;
                        _costCenter.Add(costCenter);
                        await _costCenter.SaveChangesAsync();
                    }
                    //Load Base Unit
                    sheet = workbook.GetSheetAt(workbook.GetSheetIndex("BaseUnit"));
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var baseUnit = new BaseUnits();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (headerRow.GetCell(j).ToString() == "Name") baseUnit.Name = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Value") baseUnit.Value = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Description") baseUnit.Description = row.GetCell(j).ToString();

                        }

                        baseUnit.CreatedOn = DateTimeOffset.Now;
                        baseUnit.Active = true;
                        _baseUnits.Add(baseUnit);
                        await _baseUnits.SaveChangesAsync();
                    }
                    //Load Product
                    sheet = workbook.GetSheetAt(workbook.GetSheetIndex("Products"));
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var products = new Products();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (headerRow.GetCell(j).ToString() == "Name") products.Name = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Code") products.Code = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Description") products.Description = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "BaseUnit") products.BaseUnitId = GetBaseUnitByValue(row.GetCell(j).ToString()).Id;

                        }
                        products.Active = true;
                        products.CreatedOn = DateTimeOffset.Now;
                        _products.Add(products);
                        await _products.SaveChangesAsync();
                    }
                    //Assign Products to Cost Center
                    sheet = workbook.GetSheetAt(workbook.GetSheetIndex("CostCenterProducts"));
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var costCenterProductsDetails = new CostCenterProductsDetails();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (headerRow.GetCell(j).ToString() == "CostCenterName")
                            {
                                if (_costCenterProduct.Query().FirstOrDefault(x => x.Name == "Productos para " + row.GetCell(j)) == null)
                                {
                                    var costCenterProducts = new CostCenterProducts()
                                    {
                                        Name = "Productos para " + row.GetCell(j),
                                        Active = true,
                                        CreatedOn = DateTimeOffset.Now
                                    };
                                    _costCenterProduct.Add(costCenterProducts);
                                    await _costCenterProduct.SaveChangesAsync();
                                    costCenterProductsDetails.CostCenterProductsId = costCenterProducts.Id;
                                }
                                else
                                {
                                    costCenterProductsDetails.CostCenterProductsId = _costCenterProduct.Query().FirstOrDefault(x => x.Name == "Productos para " + row.GetCell(j)).Id;
                                }
                            }
                            if (headerRow.GetCell(j).ToString() == "CostCenterName") costCenterProductsDetails.CostCenterId = GetCostCenterByName(row.GetCell(j).ToString()).Id;
                            if (headerRow.GetCell(j).ToString() == "ProductCode") costCenterProductsDetails.ProductId = GetProductByCode(row.GetCell(j).ToString()).Id;

                        }
                        _costCenterProductDetail.Add(costCenterProductsDetails);
                        await _costCenterProductDetail.SaveChangesAsync();
                    }
                }
                var error = _logger.SuccessErrorHandlingTask("Load Initial Success", "Error", "Load Initial Success", true);
                _logger.ErrorLog(error.LoggerMessage, "Load Initial", String.Empty, String.Empty);
				return true;
            }
            catch (Exception ex)
            {
                var error = _logger.SuccessErrorHandlingTask("Load Initial Failed", "Error", "Load Initial Failed", false);
                _logger.ErrorLog(error.LoggerMessage, "Load Initial", ex.InnerException.Message, String.Empty);
				return false;
            }

        }
		public IEnumerable<ConsumptionReport> ConsumptionReport(DateTime reportDate,int folio,string warehouseName )
		{
            //var testDate = reportDate.ToString("yyyyMMdd");
            //var culture = new CultureInfo("cl-ES");
            var query =
		        from cd in _consumptioDetails.Query()
		        join pr in _products.Query() on cd.ProductId equals pr.Id
		        join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
				join cp in _consumption.Query() on cd.ConsumptionId equals cp.Id
                where cp.LateCreatedOn.ToString("yyyyMMdd") == reportDate.ToString("yyyyMMdd") && cc.WarehouseCode == warehouseName
		        select new ConsumptionReport
                {
                    Warehouse = cd.WarehouseCode,
                    Folio = folio,
                    GeneratedDate = reportDate.ToString("dd/MM/yyyy"),
                    CenterCostCode = cc.Code,
                    ProductCode = pr.Code,
                    BaseUnit = cd.BaseUnit,
                    Amount = (int)cd.Weight

                };
		    var report = query
		        .GroupBy(g => new
		        {
                    g.Warehouse,
                    g.Folio,
                    g.GeneratedDate,
                    g.WarehouseOut,
                    g.Description,
                    g.CenterCostCode,
                    g.ProductCode,
                    g.BaseUnit

		        })
		        .Select(x => new ConsumptionReport
                {
                    Warehouse = x.Key.Warehouse,
                    Folio = x.Key.Folio,
                    GeneratedDate = x.Key.GeneratedDate,
                    WarehouseOut = x.Key.WarehouseOut,
                    Description = x.Key.Description,
                    CenterCostCode = x.Key.CenterCostCode,
                    ProductCode = x.Key.ProductCode,
                    BaseUnit = x.Key.BaseUnit,
		            Amount = x.Sum(o => o.Amount)
		        });
			return report;
		}
        
        public IList<WarehouseList> GetWarehouseListNames()
        {
            var warehouseList = new List<WarehouseList>();
            var warehouseNames = from cc in _costCenter.Query()
                select new { WarehouseName = cc.WarehouseCode,Selected = false };
            var report = warehouseNames.GroupBy(o => o.WarehouseName).Select(x => x.First()).ToList();
            warehouseList.Add(new WarehouseList {WarehouseName = "Select a Warehouse",Selected = true});
            foreach (var item in report)
            {
                warehouseList.Add(new WarehouseList {WarehouseName = item.WarehouseName,Selected = item.Selected});
            }

            return warehouseList;

        }

        public IList<ConsumptionList> GetConsumptionList()
        {
            var date = DateTimeOffset.Now;   
                
            var query = from c in _consumption.Query().Where(x => x.CreatedOn >= date.AddDays(-1))
                join cd in _consumptioDetails.Query() on c.Id equals cd.ConsumptionId
                join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
                select new ConsumptionList
                {
                    Id = c.Id,
                    CenterCost = cc.Name,
                    ConsumptionEfectiveEntryDate = c.LateCreatedOn.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"),
                    ConsumptionEntryDate = c.CreatedOn.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss "),
                    ConsumptionName = c.Name,
                    Warehouse = cd.WarehouseCode
                };
            var list = query
                .GroupBy(g => new
                {
                    g.Id,
                    g.CenterCost,
                    g.ConsumptionEfectiveEntryDate,
                    g.ConsumptionEntryDate,
                    g.ConsumptionName,
                    g.Warehouse
                })
                .Select(x => new ConsumptionList
                {
                    Id = x.Key.Id,
                    CenterCost = x.Key.CenterCost,
                    ConsumptionEfectiveEntryDate = x.Key.ConsumptionEfectiveEntryDate,
                    ConsumptionEntryDate = x.Key.ConsumptionEntryDate,
                    ConsumptionName = x.Key.ConsumptionName,
                    Warehouse = x.Key.Warehouse
                }).ToList();

            return list;
        }

        public Consumptions GetConsumptionById(long id)
        {
            return _consumption.Query().FirstOrDefault(x => x.Id == id);
        }

        public IList<ConsumptionDetails> ConsumptionDetailsByConsumptionId(long id)
        {
            return _consumptioDetails.Query().Where(x => x.ConsumptionId == id).ToList();
        }
        
    }
}
